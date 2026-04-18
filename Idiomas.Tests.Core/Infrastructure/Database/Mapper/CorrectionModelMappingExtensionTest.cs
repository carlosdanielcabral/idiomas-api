using Idiomas.Core.Domain.Entity;
using Idiomas.Core.Domain.Enum;
using Idiomas.Core.Infrastructure.Database.Mapper;
using Idiomas.Core.Infrastructure.Database.Model;
using Idiomas.Core.Interface.Service;
using Moq;

namespace Idiomas.Tests.Core.Infrastructure.Database.Mapper;

public class CorrectionModelMappingExtensionTest
{
    private readonly Mock<IEncryptionService> _encryptionServiceMock;

    public CorrectionModelMappingExtensionTest()
    {
        _encryptionServiceMock = new Mock<IEncryptionService>();
    }

    [Fact]
    public void ToEntity_ShouldDecryptAllFields_WhenMappingFromModel()
    {
        string encryptedOriginal = "encrypted_original";
        string encryptedSuggested = "encrypted_suggested";
        string encryptedExplanation = "encrypted_explanation";
        string decryptedOriginal = "I go yesterday";
        string decryptedSuggested = "I went yesterday";
        string decryptedExplanation = "Use past tense for completed actions";

        CorrectionModel model = CreateCorrectionModel(encryptedOriginal, encryptedSuggested, encryptedExplanation);

        _encryptionServiceMock
            .Setup(service => service.Decrypt(encryptedOriginal))
            .Returns(decryptedOriginal);

        _encryptionServiceMock
            .Setup(service => service.Decrypt(encryptedSuggested))
            .Returns(decryptedSuggested);

        _encryptionServiceMock
            .Setup(service => service.Decrypt(encryptedExplanation))
            .Returns(decryptedExplanation);

        Correction entity = model.ToEntity(_encryptionServiceMock.Object);

        Assert.Equal(decryptedOriginal, entity.OriginalFragment);
        Assert.Equal(decryptedSuggested, entity.SuggestedFragment);
        Assert.Equal(decryptedExplanation, entity.Explanation);

        _encryptionServiceMock.Verify(service => service.Decrypt(encryptedOriginal), Times.Once);
        _encryptionServiceMock.Verify(service => service.Decrypt(encryptedSuggested), Times.Once);
        _encryptionServiceMock.Verify(service => service.Decrypt(encryptedExplanation), Times.Once);
    }

    [Fact]
    public void ToEntity_ShouldMapErrorType_WhenModelHasValidErrorType()
    {
        string original = "I go yesterday";
        string suggested = "I went yesterday";
        string explanation = "Use past tense";
        CorrectionModel model = CreateCorrectionModel(original, suggested, explanation, ErrorType.Grammar);

        _encryptionServiceMock
            .Setup(service => service.Decrypt(It.IsAny<string>()))
            .Returns((string s) => s);

        Correction entity = model.ToEntity(_encryptionServiceMock.Object);

        Assert.Equal(ErrorType.Grammar, entity.Type);
    }

    [Fact]
    public void ToEntity_ShouldThrowException_WhenModelHasInvalidErrorType()
    {
        string original = "I go yesterday";
        string suggested = "I went yesterday";
        string explanation = "Use past tense";
        CorrectionModel model = CreateCorrectionModel(original, suggested, explanation);
        model.Type = "InvalidErrorType";

        _encryptionServiceMock
            .Setup(service => service.Decrypt(It.IsAny<string>()))
            .Returns((string s) => s);

        Assert.Throws<InvalidOperationException>(() => model.ToEntity(_encryptionServiceMock.Object));
    }

    [Fact]
    public void ToModel_ShouldEncryptAllFields_WhenMappingFromEntity()
    {
        string plainOriginal = "I go yesterday";
        string plainSuggested = "I went yesterday";
        string plainExplanation = "Use past tense for completed actions";
        string encryptedOriginal = "encrypted_original";
        string encryptedSuggested = "encrypted_suggested";
        string encryptedExplanation = "encrypted_explanation";

        Correction entity = CreateCorrectionEntity(plainOriginal, plainSuggested, plainExplanation);

        _encryptionServiceMock
            .Setup(service => service.Encrypt(plainOriginal))
            .Returns(encryptedOriginal);

        _encryptionServiceMock
            .Setup(service => service.Encrypt(plainSuggested))
            .Returns(encryptedSuggested);

        _encryptionServiceMock
            .Setup(service => service.Encrypt(plainExplanation))
            .Returns(encryptedExplanation);

        CorrectionModel model = entity.ToModel(_encryptionServiceMock.Object);

        Assert.Equal(encryptedOriginal, model.OriginalFragment);
        Assert.Equal(encryptedSuggested, model.SuggestedFragment);
        Assert.Equal(encryptedExplanation, model.Explanation);

        _encryptionServiceMock.Verify(service => service.Encrypt(plainOriginal), Times.Once);
        _encryptionServiceMock.Verify(service => service.Encrypt(plainSuggested), Times.Once);
        _encryptionServiceMock.Verify(service => service.Encrypt(plainExplanation), Times.Once);
    }

    [Fact]
    public void ToModel_ShouldMapErrorType_WhenEntityHasErrorType()
    {
        string original = "I go yesterday";
        string suggested = "I went yesterday";
        string explanation = "Use past tense";
        Correction entity = CreateCorrectionEntity(original, suggested, explanation, ErrorType.Vocabulary);

        _encryptionServiceMock
            .Setup(service => service.Encrypt(It.IsAny<string>()))
            .Returns("encrypted");

        CorrectionModel model = entity.ToModel(_encryptionServiceMock.Object);

        Assert.Equal(ErrorType.Vocabulary.ToString(), model.Type);
    }

    [Fact]
    public void ToEntities_ShouldMapAllModels_WhenGivenCollection()
    {
        string original = "I go yesterday";
        string suggested = "I went yesterday";
        string explanation = "Use past tense";
        List<CorrectionModel> models = new()
        {
            CreateCorrectionModel(original, suggested, explanation),
            CreateCorrectionModel(original, suggested, explanation)
        };

        _encryptionServiceMock
            .Setup(service => service.Decrypt(It.IsAny<string>()))
            .Returns((string s) => s);

        IEnumerable<Correction> entities = models.ToEntities(_encryptionServiceMock.Object);

        Assert.Equal(2, entities.Count());
    }

    private static CorrectionModel CreateCorrectionModel(
        string original,
        string suggested,
        string explanation,
        ErrorType errorType = ErrorType.Grammar)
    {
        return new CorrectionModel
        {
            Id = Guid.NewGuid(),
            MessageId = Guid.NewGuid(),
            OriginalFragment = original,
            SuggestedFragment = suggested,
            Explanation = explanation,
            Type = errorType.ToString(),
            CreatedAt = DateTime.UtcNow
        };
    }

    private static Correction CreateCorrectionEntity(
        string original,
        string suggested,
        string explanation,
        ErrorType errorType = ErrorType.Grammar)
    {
        return new Correction(
            Guid.NewGuid().ToString(),
            Guid.NewGuid().ToString(),
            original,
            suggested,
            explanation,
            errorType
        );
    }
}
