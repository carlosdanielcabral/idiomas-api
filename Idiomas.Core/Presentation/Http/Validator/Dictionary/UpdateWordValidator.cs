using System.Net;

using Idiomas.Core.Application.DTO.Dictionary;
using Idiomas.Core.Application.Error;

namespace Idiomas.Core.Presentation.Http.Validator.Dictionary;

public class UpdateWordValidator
{
    public void Validate(UpdateWordDTO dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Word))
        {
            throw new ApiException("Palavra é obrigatória", HttpStatusCode.BadRequest);
        }

        if (string.IsNullOrWhiteSpace(dto.Ipa))
        {
            throw new ApiException("IPA é obrigatório", HttpStatusCode.BadRequest);
        }

        if (dto.Meanings is null || dto.Meanings.Count == 0)
        {
            throw new ApiException("É necessário informar pelo menos um significado", HttpStatusCode.BadRequest);
        }

        this.ValidateMeanings(dto.Meanings);
    }

    private void ValidateMeanings(List<CreateMeaningDTO> meanings)
    {
        for (int index = 0; index < meanings.Count; index++)
        {
            CreateMeaningDTO meaning = meanings[index];

            if (string.IsNullOrWhiteSpace(meaning.Meaning))
            {
                throw new ApiException($"Significado na posição {index + 1} é obrigatório", HttpStatusCode.BadRequest);
            }

            if (string.IsNullOrWhiteSpace(meaning.Example))
            {
                throw new ApiException($"Exemplo na posição {index + 1} é obrigatório", HttpStatusCode.BadRequest);
            }
        }
    }
}
