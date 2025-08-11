namespace IdiomasAPI.Source.Infrastructure.Service;

public static class UUIDGenerator
{
    public static string Generate()
    {
        Guid myUuid = Guid.NewGuid();

        return myUuid.ToString();
    }
}