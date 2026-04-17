using Idiomas.Core.Domain.Entity;
using Idiomas.Core.Infrastructure.Database.Context;
using Idiomas.Core.Infrastructure.Database.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Idiomas.Tests.Core.Infrastructure.Database.Repository;

public class ScenarioRepositoryTest : IDisposable
{
    private readonly ApplicationContext _database;
    private readonly ScenarioRepository _repository;

    public ScenarioRepositoryTest()
    {
        ServiceCollection services = new();
        services.AddDbContext<ApplicationContext>(options =>
            options.UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()));

        ServiceProvider provider = services.BuildServiceProvider();
        this._database = provider.GetRequiredService<ApplicationContext>();
        this._repository = new ScenarioRepository(this._database);

        this._database.Database.EnsureCreated();
    }

    [Fact]
    public async Task GetById_WithInvalidGuidFormat_ShouldReturnNullInsteadOfThrowingException()
    {
        // Arrange
        string invalidGuid = "string";

        // Act & Assert
        Scenario? result = await this._repository.GetById(invalidGuid);
        
        // Should return null instead of throwing FormatException
        Assert.Null(result);
    }

    [Fact]
    public async Task GetById_WithValidGuid_ShouldReturnScenarioIfExists()
    {
        // Arrange
        await this._repository.SeedScenarios();
        var scenarios = await this._database.Scenario.FirstOrDefaultAsync();
        string validGuid = scenarios!.Id.ToString();

        // Act
        Scenario? result = await this._repository.GetById(validGuid);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(validGuid, result.Id);
    }

    [Fact]
    public async Task GetById_WithValidGuidButNonExistentScenario_ShouldReturnNull()
    {
        // Arrange
        string nonExistentGuid = Guid.NewGuid().ToString();

        // Act
        Scenario? result = await this._repository.GetById(nonExistentGuid);

        // Assert
        Assert.Null(result);
    }

    public void Dispose()
    {
        this._database.Database.EnsureDeleted();
        this._database.Dispose();
    }
}
