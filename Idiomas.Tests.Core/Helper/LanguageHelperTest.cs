using System.Net;
using Idiomas.Core.Application.Error;
using Idiomas.Core.Domain.Enum;
using Idiomas.Core.Helper;

namespace Idiomas.Tests.Core.Helper;

public class LanguageHelperTest
{
    [Fact]
    public void ParseLanguage_WithValidLanguage_ShouldReturnLanguage()
    {
        string language = "English";

        Language? result = LanguageHelper.ParseLanguage(language);

        Assert.Equal(Language.English, result);
    }

    [Fact]
    public void ParseLanguage_WithValidLanguage_CaseInsensitive_ShouldReturnLanguage()
    {
        string language = "english";

        Language? result = LanguageHelper.ParseLanguage(language);

        Assert.Equal(Language.English, result);
    }

    [Fact]
    public void ParseLanguage_WithNullLanguage_ShouldReturnNull()
    {
        string? language = null;

        Language? result = LanguageHelper.ParseLanguage(language);

        Assert.Null(result);
    }

    [Fact]
    public void ParseLanguage_WithEmptyLanguage_ShouldReturnNull()
    {
        string language = "";

        Language? result = LanguageHelper.ParseLanguage(language);

        Assert.Null(result);
    }

    [Fact]
    public void ParseLanguage_WithWhitespaceLanguage_ShouldReturnNull()
    {
        string language = "   ";

        Language? result = LanguageHelper.ParseLanguage(language);

        Assert.Null(result);
    }

    [Fact]
    public void ParseLanguage_WithInvalidLanguage_ShouldThrowApiException()
    {
        string language = "InvalidLanguage";

        ApiException exception = Assert.Throws<ApiException>(() => LanguageHelper.ParseLanguage(language));

        Assert.Contains("Invalid language 'InvalidLanguage'", exception.Message);
        Assert.Contains("English", exception.Message);
        Assert.Contains("Spanish", exception.Message);
        Assert.Contains("French", exception.Message);
        Assert.Contains("German", exception.Message);
        Assert.Contains("Italian", exception.Message);
        Assert.Contains("Portuguese", exception.Message);
        Assert.Equal(HttpStatusCode.BadRequest, exception.StatusCode);
    }

    [Fact]
    public void ParseLanguage_WithIsRequiredTrue_AndNullLanguage_ShouldThrowApiException()
    {
        string? language = null;

        ApiException exception = Assert.Throws<ApiException>(() => LanguageHelper.ParseLanguage(language, isRequired: true));

        Assert.Equal("Language is required.", exception.Message);
        Assert.Equal(HttpStatusCode.BadRequest, exception.StatusCode);
    }

    [Fact]
    public void ParseLanguage_WithIsRequiredTrue_AndEmptyLanguage_ShouldThrowApiException()
    {
        string language = "";

        ApiException exception = Assert.Throws<ApiException>(() => LanguageHelper.ParseLanguage(language, isRequired: true));

        Assert.Equal("Language is required.", exception.Message);
        Assert.Equal(HttpStatusCode.BadRequest, exception.StatusCode);
    }

    [Fact]
    public void ParseLanguage_WithIsRequiredTrue_AndWhitespaceLanguage_ShouldThrowApiException()
    {
        string language = "   ";

        ApiException exception = Assert.Throws<ApiException>(() => LanguageHelper.ParseLanguage(language, isRequired: true));

        Assert.Equal("Language is required.", exception.Message);
        Assert.Equal(HttpStatusCode.BadRequest, exception.StatusCode);
    }

    [Fact]
    public void ParseLanguage_WithIsRequiredTrue_AndValidLanguage_ShouldReturnLanguage()
    {
        string language = "Spanish";

        Language? result = LanguageHelper.ParseLanguage(language, isRequired: true);

        Assert.Equal(Language.Spanish, result);
    }
}
