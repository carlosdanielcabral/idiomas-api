using Idiomas.Core.Domain.Entity;
using Idiomas.Core.Domain.Enum;
using Idiomas.Core.Infrastructure.Database.Mapper;
using Idiomas.Core.Infrastructure.Database.Model;

namespace Idiomas.Tests.Core.Infrastructure.Database.Mapper;

public class ConversationMappingExtensionTest
{
    [Fact]
    public void ToEntity_ShouldMapIsActiveTrue_WhenModelIsActive()
    {
        ConversationModel model = CreateConversationModel(isActive: true);

        Conversation entity = model.ToEntity();

        Assert.True(entity.IsActive);
    }

    [Fact]
    public void ToEntity_ShouldMapIsActiveFalse_WhenModelIsInactive()
    {
        ConversationModel model = CreateConversationModel(isActive: false);

        Conversation entity = model.ToEntity();

        Assert.False(entity.IsActive);
    }

    [Fact]
    public void ToModel_ShouldMapIsActiveTrue_WhenEntityIsActive()
    {
        Conversation entity = CreateConversationEntity(isActive: true);

        ConversationModel model = entity.ToModel();

        Assert.True(model.IsActive);
    }

    [Fact]
    public void ToModel_ShouldMapIsActiveFalse_WhenEntityIsInactive()
    {
        Conversation entity = CreateConversationEntity(isActive: false);

        ConversationModel model = entity.ToModel();

        Assert.False(model.IsActive);
    }

    private static ConversationModel CreateConversationModel(bool isActive)
    {
        return new ConversationModel
        {
            Id = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            Language = Language.English.ToString(),
            Mode = ConversationMode.Free.ToString(),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            IsActive = isActive,
            Messages = new List<MessageModel>()
        };
    }

    private static Conversation CreateConversationEntity(bool isActive)
    {
        return new Conversation(
            Guid.NewGuid().ToString(),
            Guid.NewGuid().ToString(),
            Language.English,
            ConversationMode.Free,
            null,
            isActive
        );
    }
}
