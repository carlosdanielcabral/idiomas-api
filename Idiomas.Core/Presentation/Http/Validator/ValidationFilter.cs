namespace Idiomas.Core.Presentation.Http.Validator;

public class ValidationFilter<TValidator, TDto>(IServiceProvider serviceProvider) : IEndpointFilter
    where TValidator : IValidator<TDto>
    where TDto : class
{
    private readonly IServiceProvider _serviceProvider = serviceProvider;

    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        TDto? dto = context.Arguments.OfType<TDto>().FirstOrDefault();

        if (dto is not null)
        {
            TValidator validator = this._serviceProvider.GetRequiredService<TValidator>();
            validator.Validate(dto);
        }

        return await next(context);
    }
}
