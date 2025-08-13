
using IdiomasAPI.Source.Application.DTO.File;
using IdiomasAPI.Source.Interface.Controller;
using IdiomasAPI.Source.Interface.Route;

namespace IdiomasAPI.Source.Presentation.Http.Route;

public class FileRoute(IFileController controller) : IRoute
{
    private readonly IFileController _controller = controller;

    public void Register(WebApplication app)
    {
        var file = app.MapGroup("/file").RequireAuthorization();
        
        file.MapPost("/", _controller.GenerateUploadUrl)
            .Produces<CreateFileResponseDTO>(StatusCodes.Status201Created);

        file.MapPatch("/{filekey}/confirmation", _controller.ConfirmFileUpload)
            .Produces(StatusCodes.Status204NoContent);

        file.MapPatch("/{filekey}/failure", _controller.FailFileUpload)
            .Produces(StatusCodes.Status204NoContent);
    }
}
