namespace Idiomas.Core.Infrastructure.Helper;

public static class UUIDGenerator
{
    public static string Generate()
    {
        Guid myUuid = Guid.NewGuid();

        return myUuid.ToString();
    }
}