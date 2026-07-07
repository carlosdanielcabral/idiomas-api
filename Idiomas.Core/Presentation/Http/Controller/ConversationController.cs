using System.Security.Claims;
using Idiomas.Core.Application.DTO.Conversation;
using Idiomas.Core.Application.UseCase.ConversationCase;
using Idiomas.Core.Domain.Entity;
using Idiomas.Core.Domain.Enum;
using Idiomas.Core.Helper;
using Idiomas.Core.Interface.Controller;
using Idiomas.Core.Presentation.DTO.Conversation;
using Idiomas.Core.Presentation.Extensions;
using Idiomas.Core.Presentation.Mapper;

namespace Idiomas.Core.Presentation.Http.Controller;

public class ConversationController : IConversationController
{
    public async Task<IResult> StartConversation(
        CreateConversationRequestDTO dto,
        ClaimsPrincipal user,
        StartConversation useCase)
    {
        string userIdString = user.GetUserId().ToString();

        Language language = LanguageHelper.ParseLanguage(dto.Language, isRequired: true)
            ?? throw new InvalidOperationException("Language cannot be null when isRequired is true.");

        StartConversationRequest request = new(language, dto.Mode, dto.ScenarioId);

        Conversation conversation = await useCase.Execute(request, userIdString);

        ConversationResponseDTO response = conversation.ToResponseDTO();

        return TypedResults.Created($"/api/conversations/{conversation.Id}", response);
    }

    public async Task<IResult> SendMessage(
        string conversationId,
        SendMessageRequestDTO dto,
        ClaimsPrincipal user,
        SendMessage useCase)
    {
        string userIdString = user.GetUserId().ToString();

        SendMessageRequest request = new(dto.Content);

        MessageResponse messageResponse = await useCase.Execute(conversationId, request, userIdString);

        MessageResponseDTO response = new()
        {
            Id = messageResponse.Id,
            Content = messageResponse.Content,
            Role = messageResponse.Role,
            Corrections = messageResponse.Corrections.Select(c => new CorrectionResponseDTO
            {
                OriginalFragment = c.OriginalFragment,
                SuggestedFragment = c.SuggestedFragment,
                Explanation = c.Explanation,
                Type = c.Type
            }).ToList(),
            CreatedAt = messageResponse.CreatedAt
        };

        return TypedResults.Ok(response);
    }

    public async Task<IResult> ListScenarios(string? language, ListScenarios useCase)
    {
        IEnumerable<Scenario> scenarios = await useCase.Execute(language);

        List<ScenarioResponseDTO> response = scenarios.Select(scenario => new ScenarioResponseDTO
        {
            Id = scenario.Id,
            Title = scenario.Title,
            Description = scenario.Description
        }).ToList();

        return TypedResults.Ok(response);
    }

    public async Task<IResult> GetConversation(
        string conversationId,
        ClaimsPrincipal user,
        GetConversation useCase)
    {
        string userIdString = user.GetUserId().ToString();

        Conversation conversation = await useCase.Execute(conversationId, userIdString);

        ConversationDetailResponseDTO response = new()
        {
            Id = conversation.Id,
            Language = conversation.Language,
            Mode = conversation.Mode,
            ScenarioId = conversation.ScenarioId,
            CreatedAt = conversation.CreatedAt,
            UpdatedAt = conversation.UpdatedAt,
            IsActive = conversation.IsActive,
            Messages = conversation.Messages.Select(m => new MessageResponseDTO
            {
                Id = m.Id,
                Content = m.Content,
                Role = m.Role,
                Corrections = m.Corrections.Select(c => new CorrectionResponseDTO
                {
                    OriginalFragment = c.OriginalFragment,
                    SuggestedFragment = c.SuggestedFragment,
                    Explanation = c.Explanation,
                    Type = c.Type
                }).ToList(),
                CreatedAt = m.CreatedAt
            }).ToList()
        };

        return TypedResults.Ok(response);
    }

    public async Task<IResult> ListConversations(ClaimsPrincipal user, ListConversations useCase)
    {
        string userIdString = user.GetUserId().ToString();

        IEnumerable<Conversation> conversations = await useCase.Execute(userIdString);

        ConversationListWrapperDTO response = new()
        {
            Conversations = conversations.Select(c => c.ToListResponseDTO()).ToList()
        };

        return TypedResults.Ok(response);
    }

    public async Task<IResult> EndConversation(
        string conversationId,
        ClaimsPrincipal user,
        EndConversation useCase)
    {
        string userIdString = user.GetUserId().ToString();

        await useCase.Execute(conversationId, userIdString);

        return TypedResults.NoContent();
    }
}
