using Idiomas.Core.Domain.Entity;
using Idiomas.Core.Domain.Enum;
using Idiomas.Core.Infrastructure.Database.Context;
using Idiomas.Core.Infrastructure.Database.Mapper;
using Idiomas.Core.Infrastructure.Database.Model;
using Idiomas.Core.Interface.Repository;
using Microsoft.EntityFrameworkCore;

namespace Idiomas.Core.Infrastructure.Database.Repository;

public class ScenarioRepository(ApplicationContext database) : IScenarioRepository
{
    private readonly ApplicationContext _database = database;

    public async Task<IEnumerable<Scenario>> GetByLanguage(Language language)
    {
        List<ScenarioModel> models = await this._database.Scenario
            .Where(s => s.Language == language.ToString() && s.IsActive)
            .ToListAsync();

        return models.ToEntities();
    }

    public async Task<Scenario?> GetById(string id)
    {
        Guid scenarioId = Guid.Parse(id);

        ScenarioModel? model = await this._database.Scenario
            .FirstOrDefaultAsync(s => s.Id == scenarioId);

        return model?.ToEntity();
    }

    public async Task SeedScenarios()
    {
        bool hasScenarios = await this._database.Scenario.AnyAsync();

        if (hasScenarios)
        {
            return;
        }

        List<ScenarioModel> defaultScenarios = new()
        {
            // English
            new ScenarioModel { Id = Guid.NewGuid(), Language = "English", Title = "At the Restaurant", Description = "Ordering food and drinks at a restaurant", IsActive = true },
            new ScenarioModel { Id = Guid.NewGuid(), Language = "English", Title = "Hotel Check-in", Description = "Checking into a hotel and discussing room preferences", IsActive = true },
            new ScenarioModel { Id = Guid.NewGuid(), Language = "English", Title = "Job Interview", Description = "A professional job interview scenario", IsActive = true },
            new ScenarioModel { Id = Guid.NewGuid(), Language = "English", Title = "Shopping", Description = "Buying clothes and asking about sizes and prices", IsActive = true },
            new ScenarioModel { Id = Guid.NewGuid(), Language = "English", Title = "Doctor's Appointment", Description = "Describing symptoms and discussing health", IsActive = true },

            // Spanish
            new ScenarioModel { Id = Guid.NewGuid(), Language = "Spanish", Title = "En el Restaurante", Description = "Ordering food at a Spanish restaurant", IsActive = true },
            new ScenarioModel { Id = Guid.NewGuid(), Language = "Spanish", Title = "En el Hotel", Description = "Hotel check-in scenario in Spanish", IsActive = true },
            new ScenarioModel { Id = Guid.NewGuid(), Language = "Spanish", Title = "Entrevista de Trabajo", Description = "Job interview in Spanish", IsActive = true },
            new ScenarioModel { Id = Guid.NewGuid(), Language = "Spanish", Title = "De Compras", Description = "Shopping scenario in Spanish", IsActive = true },
            new ScenarioModel { Id = Guid.NewGuid(), Language = "Spanish", Title = "En el Médico", Description = "Doctor's appointment in Spanish", IsActive = true },

            // French
            new ScenarioModel { Id = Guid.NewGuid(), Language = "French", Title = "Au Restaurant", Description = "Ordering food at a French restaurant", IsActive = true },
            new ScenarioModel { Id = Guid.NewGuid(), Language = "French", Title = "À l'Hôtel", Description = "Hotel check-in in French", IsActive = true },
            new ScenarioModel { Id = Guid.NewGuid(), Language = "French", Title = "Entretien d'Embauche", Description = "Job interview in French", IsActive = true },
            new ScenarioModel { Id = Guid.NewGuid(), Language = "French", Title = "Shopping", Description = "Shopping scenario in French", IsActive = true },
            new ScenarioModel { Id = Guid.NewGuid(), Language = "French", Title = "Chez le Médecin", Description = "Doctor's appointment in French", IsActive = true },

            // German
            new ScenarioModel { Id = Guid.NewGuid(), Language = "German", Title = "Im Restaurant", Description = "Ordering food at a German restaurant", IsActive = true },
            new ScenarioModel { Id = Guid.NewGuid(), Language = "German", Title = "Im Hotel", Description = "Hotel check-in in German", IsActive = true },
            new ScenarioModel { Id = Guid.NewGuid(), Language = "German", Title = "Vorstellungsgespräch", Description = "Job interview in German", IsActive = true },
            new ScenarioModel { Id = Guid.NewGuid(), Language = "German", Title = "Einkaufen", Description = "Shopping scenario in German", IsActive = true },
            new ScenarioModel { Id = Guid.NewGuid(), Language = "German", Title = "Beim Arzt", Description = "Doctor's appointment in German", IsActive = true },

            // Italian
            new ScenarioModel { Id = Guid.NewGuid(), Language = "Italian", Title = "Al Ristorante", Description = "Ordering food at an Italian restaurant", IsActive = true },
            new ScenarioModel { Id = Guid.NewGuid(), Language = "Italian", Title = "In Hotel", Description = "Hotel check-in in Italian", IsActive = true },
            new ScenarioModel { Id = Guid.NewGuid(), Language = "Italian", Title = "Colloquio di Lavoro", Description = "Job interview in Italian", IsActive = true },
            new ScenarioModel { Id = Guid.NewGuid(), Language = "Italian", Title = "Shopping", Description = "Shopping scenario in Italian", IsActive = true },
            new ScenarioModel { Id = Guid.NewGuid(), Language = "Italian", Title = "Dal Dottore", Description = "Doctor's appointment in Italian", IsActive = true },

            // Portuguese
            new ScenarioModel { Id = Guid.NewGuid(), Language = "Portuguese", Title = "No Restaurante", Description = "Ordering food at a Portuguese restaurant", IsActive = true },
            new ScenarioModel { Id = Guid.NewGuid(), Language = "Portuguese", Title = "No Hotel", Description = "Hotel check-in in Portuguese", IsActive = true },
            new ScenarioModel { Id = Guid.NewGuid(), Language = "Portuguese", Title = "Entrevista de Emprego", Description = "Job interview in Portuguese", IsActive = true },
            new ScenarioModel { Id = Guid.NewGuid(), Language = "Portuguese", Title = "Fazendo Compras", Description = "Shopping scenario in Portuguese", IsActive = true },
            new ScenarioModel { Id = Guid.NewGuid(), Language = "Portuguese", Title = "No Médico", Description = "Doctor's appointment in Portuguese", IsActive = true }
        };

        await this._database.Scenario.AddRangeAsync(defaultScenarios);
        await this._database.SaveChangesAsync();
    }
}
