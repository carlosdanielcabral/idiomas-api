using Idiomas.Core.Application.DTO.Conversation;
using Idiomas.Core.Application.Error;
using Idiomas.Core.Domain.Entity;
using Idiomas.Core.Infrastructure.Helper;
using Idiomas.Core.Interface.Repository;
using System.Net;

namespace Idiomas.Core.Application.UseCase.ConversationCase;

public class StartConversation(
    IConversationRepository conversationRepository,
    IScenarioRepository scenarioRepository)
{
    private readonly IConversationRepository _conversationRepository = conversationRepository;
    private readonly IScenarioRepository _scenarioRepository = scenarioRepository;

    public async Task<Conversation> Execute(StartConversationRequest request, string userId)
    {
        string conversationId = UUIDGenerator.Generate();

        Conversation conversation = new(conversationId, userId, request.Language, request.Mode);

        if (!string.IsNullOrEmpty(request.ScenarioId))
        {
            Scenario? scenario = await this._scenarioRepository.GetById(request.ScenarioId);
            if (scenario == null)
            {
                throw new ApiException("Scenario not found.", HttpStatusCode.NotFound);
            }

            if (scenario.Language != request.Language)
            {
                throw new ApiException(
                    "Scenario language does not match conversation language.",
                    HttpStatusCode.BadRequest);
            }

            conversation.SetScenarioId(request.ScenarioId);
        }

        return await this._conversationRepository.Insert(conversation);
    }
}
