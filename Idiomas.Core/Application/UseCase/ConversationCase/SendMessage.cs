using Idiomas.Core.Application.DTO.Conversation;
using Idiomas.Core.Application.Error;
using Idiomas.Core.Domain.Entity;
using Idiomas.Core.Domain.Enum;
using Idiomas.Core.Infrastructure.Helper;
using Idiomas.Core.Interface.Repository;
using Idiomas.Core.Interface.Service;
using System.Net;

namespace Idiomas.Core.Application.UseCase.ConversationCase;

public class SendMessage(
    IConversationRepository conversationRepository,
    IScenarioRepository scenarioRepository,
    IConversationLLMService llmService)
{
    private readonly IConversationRepository _conversationRepository = conversationRepository;
    private readonly IScenarioRepository _scenarioRepository = scenarioRepository;
    private readonly IConversationLLMService _llmService = llmService;

    public async Task<MessageResponse> Execute(string conversationId, SendMessageRequest request, string userId)
    {
        this.ValidateRequest(request);

        Conversation conversation = await this.GetAndValidateConversation(conversationId, userId);
        
        await this.SaveUserMessage(conversation, request.Content);

        string? scenarioDescription = await this.GetScenarioDescription(conversation.ScenarioId);
        
        ConversationLLMResponse llmResponse = await this._llmService.SendMessageAsync(
            conversation,
            request.Content,
            scenarioDescription
        );

        List<CorrectionResponse> correctionResponses = await this.ProcessCorrections(
            llmResponse.Corrections,
            conversation.Messages.Last(message => message.Role == MessageRole.User).Id
        );

        Message assistantMessage = await this.SaveAssistantMessage(conversationId, llmResponse.Content);

        return this.BuildMessageResponse(assistantMessage, correctionResponses);
    }

    private void ValidateRequest(SendMessageRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Content))
        {
            throw new ApiException("Message content cannot be empty", HttpStatusCode.BadRequest);
        }
    }

    private async Task<Conversation> GetAndValidateConversation(string conversationId, string userId)
    {
        Conversation? conversation = await this._conversationRepository.GetById(conversationId);

        if (conversation == null)
        {
            throw new ApiException(
                $"Conversation with ID {conversationId} not found.",
                HttpStatusCode.NotFound);
        }

        if (conversation.UserId != userId)
        {
            throw new ApiException(
                "You do not have permission to access this conversation.",
                HttpStatusCode.Forbidden);
        }

        if (!conversation.IsActive)
        {
            throw new ApiException(
                "This conversation has ended.",
                HttpStatusCode.Conflict);
        }

        return conversation;
    }

    private async Task SaveUserMessage(Conversation conversation, string content)
    {
        string messageId = UUIDGenerator.Generate();
        this.ValidateMessage(messageId, conversation.Id, content);

        Message message = new(messageId, conversation.Id, MessageRole.User, content);

        await this._conversationRepository.InsertMessage(message);
        conversation.AddMessage(message);
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
        foreach (CorrectionResponse correctionResponse in corrections)
        {
            this.ValidateCorrection(correctionResponse, userMessageId);

            string correctionId = UUIDGenerator.Generate();
            Correction correction = new(
                correctionId,
                userMessageId,
                correctionResponse.OriginalFragment,
                correctionResponse.SuggestedFragment,
                correctionResponse.Explanation,
                correctionResponse.Type
            );

            await this._conversationRepository.InsertCorrection(correction);
        }

        return corrections;
    }

    private void ValidateCorrection(CorrectionResponse correction, string userMessageId)
    {
        const string ERROR_MESSAGE = "Failed to load response from AI";

        if (string.IsNullOrWhiteSpace(userMessageId))
        {
            throw new ApiException(ERROR_MESSAGE, HttpStatusCode.InternalServerError);
        }

        if (string.IsNullOrWhiteSpace(correction.OriginalFragment))
        {
            throw new ApiException(ERROR_MESSAGE, HttpStatusCode.InternalServerError);
        }

        if (string.IsNullOrWhiteSpace(correction.SuggestedFragment))
        {
            throw new ApiException(ERROR_MESSAGE, HttpStatusCode.InternalServerError);
        }

        if (string.IsNullOrWhiteSpace(correction.Explanation))
        {
            throw new ApiException(ERROR_MESSAGE, HttpStatusCode.InternalServerError);
        }
    }

    private async Task<Message> SaveAssistantMessage(string conversationId, string content)
    {
        string messageId = UUIDGenerator.Generate();
        this.ValidateMessage(messageId, conversationId, content);

        Message message = new(messageId, conversationId, MessageRole.Assistant, content);

        await this._conversationRepository.InsertMessage(message);

        return message;
    }

    private void ValidateMessage(string messageId, string conversationId, string content)
    {
        const string ERROR_MESSAGE = "Failed to create message";

        if (string.IsNullOrWhiteSpace(messageId))
        {
            throw new ApiException(ERROR_MESSAGE, HttpStatusCode.InternalServerError);
        }

        if (string.IsNullOrWhiteSpace(conversationId))
        {
            throw new ApiException(ERROR_MESSAGE, HttpStatusCode.InternalServerError);
        }

        if (string.IsNullOrWhiteSpace(content))
        {
            throw new ApiException(ERROR_MESSAGE, HttpStatusCode.InternalServerError);
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
