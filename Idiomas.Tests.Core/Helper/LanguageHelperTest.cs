using System.Net;
using Idiomas.Core.Helper.Error;
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
    public void ParseLanguage_WithInvalidLanguage_ShouldThrowLanguageInvalidException()
    {
        string language = "InvalidLanguage";

        LanguageInvalidException exception = Assert.Throws<LanguageInvalidException>(() => LanguageHelper.ParseLanguage(language));

        Assert.Equal(HttpStatusCode.BadRequest, exception.StatusCode);
        Assert.Equal("common:language-invalid", exception.ErrorCode);
        Assert.Equal("Language invalid", exception.Title);
        Assert.Contains("InvalidLanguage", exception.Detail);
        Assert.Contains("English", exception.Detail);
        Assert.Contains("Spanish", exception.Detail);
        Assert.Contains("French", exception.Detail);
        Assert.Contains("German", exception.Detail);
        Assert.Contains("Italian", exception.Detail);
        Assert.Contains("Portuguese", exception.Detail);
    }

    [Fact]
    public void ParseLanguage_WithIsRequiredTrue_AndNullLanguage_ShouldThrowLanguageRequiredException()
    {
        string? language = null;

        LanguageRequiredException exception = Assert.Throws<LanguageRequiredException>(() => LanguageHelper.ParseLanguage(language, isRequired: true));

        Assert.Equal(HttpStatusCode.BadRequest, exception.StatusCode);
        Assert.Equal("common:language-required", exception.ErrorCode);
        Assert.Equal("Language required", exception.Title);
        Assert.Equal("A language must be specified.", exception.Detail);
    }

    [Fact]
    public void ParseLanguage_WithIsRequiredTrue_AndEmptyLanguage_ShouldThrowLanguageRequiredException()
    {
        string language = "";

        LanguageRequiredException exception = Assert.Throws<LanguageRequiredException>(() => LanguageHelper.ParseLanguage(language, isRequired: true));

        Assert.Equal(HttpStatusCode.BadRequest, exception.StatusCode);
        Assert.Equal("common:language-required", exception.ErrorCode);
    }

    [Fact]
    public void ParseLanguage_WithIsRequiredTrue_AndWhitespaceLanguage_ShouldThrowLanguageRequiredException()
    {
        string language = "   ";

        LanguageRequiredException exception = Assert.Throws<LanguageRequiredException>(() => LanguageHelper.ParseLanguage(language, isRequired: true));

        Assert.Equal(HttpStatusCode.BadRequest, exception.StatusCode);
        Assert.Equal("common:language-required", exception.ErrorCode);
    }

    [Fact]
    public void ParseLanguage_WithIsRequiredTrue_AndValidLanguage_ShouldReturnLanguage()
    {
        string language = "Spanish";

        Language? result = LanguageHelper.ParseLanguage(language, isRequired: true);

        Assert.Equal(Language.Spanish, result);
    }
}
