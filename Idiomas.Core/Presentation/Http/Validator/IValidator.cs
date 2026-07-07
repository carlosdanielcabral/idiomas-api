namespace Idiomas.Core.Presentation.Http.Validator;

public interface IValidator<TDto>
{
    void Validate(TDto dto);
}
