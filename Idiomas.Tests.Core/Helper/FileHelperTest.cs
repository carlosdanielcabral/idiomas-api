using System.Text.RegularExpressions;
using Idiomas.Core.Helper;

namespace Idiomas.Tests.Core.Helper;

public class FileHelperTests
{
    private readonly FileHelper _sut; // System Under Test

    public FileHelperTests()
    {
        _sut = new FileHelper();
    }

    [Fact]
    public void GenerateFileKey_ShouldReturnKeyInExpectedFormat()
    {
        var originalFilename = "documento-de-teste.pdf";
        var expectedExtension = ".pdf";
        
        // O formato esperado é: 14 caracteres de timestamp + 32 caracteres de GUID + a extensão.
        var expectedLength = 14 + 32 + expectedExtension.Length;

        var fileKey = _sut.GenerateFileKey(originalFilename);


        // 1. Verifica o comprimento total da chave
        Assert.Equal(expectedLength, fileKey.Length);

        // 2. Verifica se a chave termina com a extensão correta
        Assert.EndsWith(expectedExtension, fileKey);

        // 3. Verifica se a chave começa com um padrão de data/hora válido (ex: 20250813235959)
        // Usamos uma expressão regular para validar o formato YYYYMMDDHHMMSS no início da string.
        var timestampRegex = new Regex(@"^\d{14}"); // Procura por 14 dígitos no início
        Assert.Matches(timestampRegex, fileKey);
    }
}