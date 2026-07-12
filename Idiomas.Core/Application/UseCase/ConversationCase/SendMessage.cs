using Idiomas.Core.Application.DTO.Conversation;
using Idiomas.Core.Application.Exceptions.Conversation;
using Idiomas.Core.Domain.Entity;
using Idiomas.Core.Domain.Enum;
using Idiomas.Core.Interface.Repository;
using Idiomas.Core.Interface.Service;

namespace Idiomas.Core.Application.UseCase.ConversationCase;

public class SendMessage(
    IConversationRepository conversationRepository,
    IScenarioRepository scenarioRepository,
    IConversationLLMService llmService,
    IUnitOfWork unitOfWork)
{
    private readonly IConversationRepository _conversationRepository = conversationRepository;
    private readonly IScenarioRepository _scenarioRepository = scenarioRepository;
    private readonly IConversationLLMService _llmService = llmService;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<MessageResponse> Execute(string conversationId, SendMessageRequest request, string userId)
    {
        Conversation conversation = await this.GetAndValidateConversation(conversationId, userId);

        Message userMessage = this.CreateUserMessage(conversation, request.Content);

        string? scenarioDescription = await this.GetScenarioDescription(conversation.ScenarioId);

        ConversationLLMResponse llmResponse = await this._llmService.SendMessageAsync(
            conversation,
            request.Content,
            scenarioDescription
        );

        (List<CorrectionResponse> correctionResponses, Message assistantMessage) = await this._unitOfWork.ExecuteAsync(async () =>
        {
            await this._conversationRepository.InsertMessage(userMessage);

            List<CorrectionResponse> correctionResponses = await this.ProcessCorrections(
                llmResponse.Corrections,
                userMessage.Id
            );

            Message assistantMessage = await this.SaveAssistantMessage(conversationId, llmResponse.Content);

            return (correctionResponses, assistantMessage);
        });

        return this.BuildMessageResponse(assistantMessage, correctionResponses);
    }

    private async Task<Conversation> GetAndValidateConversation(string conversationId, string userId)
    {
        Conversation? conversation = await this._conversationRepository.GetById(conversationId);

        if (conversation == null)
        {
            throw new ConversationNotFoundException();
        }

        if (conversation.UserId != userId)
        {
            throw new ConversationAccessDeniedException();
        }

        if (!conversation.IsActive)
        {
            throw new ConversationEndedException();
        }

        return conversation;
    }

    private Message CreateUserMessage(Conversation conversation, string content)
    {
        this.ValidateMessageContent(content);

        Message message = Message.Create(conversation.Id, MessageRole.User, content);

        conversation.AddMessage(message);

        return message;
    }

    private async Task<string?> GetScenarioDescription(string? scenarioId)
    {
        if (string.IsNullOrEmpty(scenarioId))
        {
            return null;
        }

        Scenario? scenario = await this._scenarioRepository.GetById(scenarioId);

        return scenario?.Description;
    }

    private async Task<List<CorrectionResponse>> ProcessCorrections(
        List<CorrectionResponse> corrections,
        string userMessageId)
    {
        List<CorrectionResponse> validCorrections = new();

        foreach (CorrectionResponse correctionResponse in corrections)
        {
            Correction? correction = Correction.Create(
                userMessageId,
                correctionResponse.OriginalFragment,
                correctionResponse.SuggestedFragment,
                correctionResponse.Explanation,
                correctionResponse.Type
            );

            if (correction is not null)
            {
                await this._conversationRepository.InsertCorrection(correction);

                validCorrections.Add(correctionResponse);
            }
        }

        return validCorrections;
    }

    private async Task<Message> SaveAssistantMessage(string conversationId, string content)
    {
        this.ValidateMessageContent(content);

        Message message = Message.Create(conversationId, MessageRole.Assistant, content);

        await this._conversationRepository.InsertMessage(message);

        return message;
    }

    private void ValidateMessageContent(string content)
    {
        if (string.IsNullOrWhiteSpace(content))
        {
            throw new MessageCreationFailedException();
        }
    }

    private MessageResponse BuildMessageResponse(Message assistantMessage, List<CorrectionResponse> corrections)
    {
        return new MessageResponse(
            assistantMessage.Id,
            assistantMessage.Content,
            assistantMessage.Role,
            corrections,
            assistantMessage.CreatedAt
        );
    }
}
