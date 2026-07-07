namespace Idiomas.Core.Presentation.Http.Validator;

public static class RouteHandlerBuilderExtensions
{
    public static RouteHandlerBuilder WithValidation<TValidator, TDto>(this RouteHandlerBuilder builder)
        where TValidator : IValidator<TDto>
        where TDto : class
    {
        return builder.AddEndpointFilter<ValidationFilter<TValidator, TDto>>();
    }
}
