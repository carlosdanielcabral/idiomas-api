using Idiomas.Core.Application.DTO.Conversation;
using Idiomas.Core.Application.Error;
using Idiomas.Core.Application.Mapper;
using Idiomas.Core.Domain.Entity;
using Idiomas.Core.Domain.Enum;
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
        await this.ValidateConversation(request, userId);

        Conversation conversation = request.ToEntity(userId, request.ScenarioId);

        return await this._conversationRepository.Insert(conversation);
    }

    private async Task ValidateConversation(StartConversationRequest request, string userId)
    {
        if (!request.Mode.RequiresScenario())
        {
            return;
        }

        Scenario? scenario = await this._scenarioRepository.GetById(request.ScenarioId!);

        if (scenario == null)
        {
            throw new ApiException("Scenario not found.", HttpStatusCode.NotFound);
        }

        if (!scenario.MatchesLanguage(request.Language))
        {
            throw new ApiException(
                "Scenario language does not match conversation language.",
                HttpStatusCode.BadRequest);
        }
    }
}
