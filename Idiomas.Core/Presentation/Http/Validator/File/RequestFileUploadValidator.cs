using System.Text.RegularExpressions;
using Idiomas.Application.DTO.File;
using Idiomas.Core.Exceptions.Validation;

namespace Idiomas.Core.Presentation.Http.Validator.File;

public partial class RequestFileUploadValidator : IValidator<CreateFileDTO>
{
    public void Validate(CreateFileDTO dto)
    {
        if (string.IsNullOrWhiteSpace(dto.OriginalFilename))
        {
            throw new FieldRequiredException("originalFilename");
        }

        if (string.IsNullOrWhiteSpace(dto.MimeType))
        {
            throw new FieldRequiredException("mimeType");
        }

        if (!MimeTypeRegex().IsMatch(dto.MimeType))
        {
            throw new FieldInvalidException("mimeType");
        }

        if (dto.Size <= 0)
        {
            throw new NumberMustBePositiveException("size");
        }
    }

    [GeneratedRegex(@"^[a-zA-Z0-9][a-zA-Z0-9!#$&\-^_]*\/[a-zA-Z0-9][a-zA-Z0-9!#$&\-^_.+]*$")]
    private static partial Regex MimeTypeRegex();
}
