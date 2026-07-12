using Idiomas.Core.Application.DTO.Dictionary;
using Idiomas.Core.Exceptions.Validation;

namespace Idiomas.Core.Presentation.Http.Validator.Dictionary;

public class UpdateWordValidator : IValidator<UpdateWordDTO>
{
    public void Validate(UpdateWordDTO dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Word))
        {
            throw new FieldRequiredException("word");
        }

        if (string.IsNullOrWhiteSpace(dto.Ipa))
        {
            throw new FieldRequiredException("ipa");
        }

        if (dto.Meanings is null || dto.Meanings.Count == 0)
        {
            throw new FieldRequiredException("meanings");
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
                throw new ItemAtPositionRequiredException("meaning", index + 1);
            }

            if (string.IsNullOrWhiteSpace(meaning.Example))
            {
                throw new ItemAtPositionRequiredException("example", index + 1);
            }
        }
    }
}
