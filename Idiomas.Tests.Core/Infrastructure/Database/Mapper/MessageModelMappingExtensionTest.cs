using Idiomas.Core.Domain.Entity;
using Idiomas.Core.Domain.Enum;
using Idiomas.Core.Infrastructure.Database.Mapper;
using Idiomas.Core.Infrastructure.Database.Model;
using Idiomas.Core.Interface.Service;
using Moq;

namespace Idiomas.Tests.Core.Infrastructure.Database.Mapper;

public class MessageModelMappingExtensionTest
{
    private readonly Mock<IEncryptionService> _encryptionServiceMock;

    public MessageModelMappingExtensionTest()
    {
        _encryptionServiceMock = new Mock<IEncryptionService>();
    }

    [Fact]
    public void ToEntity_ShouldDecryptContent_WhenMappingFromModel()
    {
        string encryptedContent = "encrypted_content";
        string decryptedContent = "Hello, how are you?";
        MessageModel model = CreateMessageModel(encryptedContent);

        _encryptionServiceMock
            .Setup(service => service.Decrypt(encryptedContent))
            .Returns(decryptedContent);

        Message entity = model.ToEntity(_encryptionServiceMock.Object);

        Assert.Equal(decryptedContent, entity.Content);
        _encryptionServiceMock.Verify(service => service.Decrypt(encryptedContent), Times.Once);
    }

    [Fact]
    public void ToEntity_ShouldMapRole_WhenModelHasValidRole()
    {
        string content = "Hello";
        MessageModel model = CreateMessageModel(content, MessageRole.User);

        _encryptionServiceMock
            .Setup(service => service.Decrypt(It.IsAny<string>()))
            .Returns(content);

        Message entity = model.ToEntity(_encryptionServiceMock.Object);

        Assert.Equal(MessageRole.User, entity.Role);
    }

    [Fact]
    public void ToEntity_ShouldThrowException_WhenModelHasInvalidRole()
    {
        string content = "Hello";
        MessageModel model = CreateMessageModel(content);
        model.Role = "InvalidRole";

        _encryptionServiceMock
            .Setup(service => service.Decrypt(It.IsAny<string>()))
            .Returns(content);

        Assert.Throws<InvalidOperationException>(() => model.ToEntity(_encryptionServiceMock.Object));
    }

    [Fact]
    public void ToEntity_ShouldMapCorrections_WhenModelHasCorrections()
    {
        string content = "I go yesterday";
        MessageModel model = CreateMessageModel(content);
        model.Corrections = new List<CorrectionModel>
        {
            CreateCorrectionModel("I go yesterday", "I went yesterday")
        };

        _encryptionServiceMock
            .Setup(service => service.Decrypt(It.IsAny<string>()))
            .Returns(content);

        Message entity = model.ToEntity(_encryptionServiceMock.Object);

        Assert.Single(entity.Corrections);
    }

    [Fact]
    public void ToModel_ShouldEncryptContent_WhenMappingFromEntity()
    {
        string plainContent = "Hello, how are you?";
        string encryptedContent = "encrypted_content";
        Message entity = CreateMessageEntity(plainContent);

        _encryptionServiceMock
            .Setup(service => service.Encrypt(plainContent))
            .Returns(encryptedContent);

        MessageModel model = entity.ToModel(_encryptionServiceMock.Object);

        Assert.Equal(encryptedContent, model.Content);
        _encryptionServiceMock.Verify(service => service.Encrypt(plainContent), Times.Once);
    }

    [Fact]
    public void ToModel_ShouldMapRole_WhenEntityHasRole()
    {
        string content = "Hello";
        Message entity = CreateMessageEntity(content, MessageRole.Assistant);

        _encryptionServiceMock
            .Setup(service => service.Encrypt(It.IsAny<string>()))
            .Returns("encrypted");

        MessageModel model = entity.ToModel(_encryptionServiceMock.Object);

        Assert.Equal(MessageRole.Assistant.ToString(), model.Role);
    }

    [Fact]
    public void ToEntities_ShouldMapAllModels_WhenGivenCollection()
    {
        string content1 = "Hello";
        string content2 = "Hi there";
        List<MessageModel> models = new()
        {
            CreateMessageModel(content1),
            CreateMessageModel(content2)
        };

        _encryptionServiceMock
            .Setup(service => service.Decrypt(It.IsAny<string>()))
            .Returns((string s) => s);

        IEnumerable<Message> entities = models.ToEntities(_encryptionServiceMock.Object);

        Assert.Equal(2, entities.Count());
    }

    private static MessageModel CreateMessageModel(string content, MessageRole role = MessageRole.User)
    {
        return new MessageModel
        {
            Id = Guid.NewGuid(),
            ConversationId = Guid.NewGuid(),
            Role = role.ToString(),
            Content = content,
            CreatedAt = DateTime.UtcNow,
            Corrections = new List<CorrectionModel>()
        };
    }

    private static CorrectionModel CreateCorrectionModel(string original, string suggested)
    {
        return new CorrectionModel
        {
            Id = Guid.NewGuid(),
            MessageId = Guid.NewGuid(),
            OriginalFragment = original,
            SuggestedFragment = suggested,
            Explanation = "Use past tense",
            Type = ErrorType.Grammar.ToString(),
            CreatedAt = DateTime.UtcNow
        };
    }

    private static Message CreateMessageEntity(string content, MessageRole role = MessageRole.User)
    {
        return new Message(
            Guid.NewGuid().ToString(),
            Guid.NewGuid().ToString(),
            role,
            content
        );
    }
}
