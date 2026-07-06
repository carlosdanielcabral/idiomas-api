using System.Net;
using System.Text.RegularExpressions;

using Idiomas.Application.DTO.File;
using Idiomas.Core.Application.Error;

namespace Idiomas.Core.Presentation.Http.Validator.File;

public partial class RequestFileUploadValidator
{
    public void Validate(CreateFileDTO dto)
    {
        if (string.IsNullOrWhiteSpace(dto.OriginalFilename))
        {
            throw new ApiException("Nome do arquivo é obrigatório", HttpStatusCode.BadRequest);
        }

        if (string.IsNullOrWhiteSpace(dto.MimeType))
        {
            throw new ApiException("Tipo do arquivo é obrigatório", HttpStatusCode.BadRequest);
        }

        if (!MimeTypeRegex().IsMatch(dto.MimeType))
        {
            throw new ApiException("Tipo do arquivo inválido", HttpStatusCode.BadRequest);
        }

        if (dto.Size <= 0)
        {
            throw new ApiException("Tamanho do arquivo deve ser maior que zero", HttpStatusCode.BadRequest);
        }
    }

    [GeneratedRegex(@"^[a-zA-Z0-9][a-zA-Z0-9!#$&\-^_]*\/[a-zA-Z0-9][a-zA-Z0-9!#$&\-^_.+]*$")]
    private static partial Regex MimeTypeRegex();
}
