namespace Idiomas.Core.Presentation.Http.Middleware;

public static class ProblemDetailsUris
{
    public const string ErrorTypePrefix = "tag:idiomas.api,2026:error:";
    public const string InstancePrefix = "tag:idiomas.api,2026:trace:";

    public static string ErrorType(string errorCode) =>
        $"{ErrorTypePrefix}{errorCode}";

    public static string Instance(string traceIdentifier) =>
        $"{InstancePrefix}{traceIdentifier}";
}
