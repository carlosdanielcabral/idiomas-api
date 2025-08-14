using Idiomas.Source.Domain.Entity;
using Idiomas.Source.Infrastructure.Database.Model;

namespace Idiomas.Source.Infrastructure.Database.Mapper;

public static class UserMappingExtension
{
    public static User ToEntity(this UserModel model)
    {
        return new User(model.Id.ToString(), model.Name, model.Email, model.Password);
    }

    public static UserModel ToModel(this User entity)
    {
        return new UserModel() { Id = Guid.Parse(entity.Id), Name = entity.Name, Email = entity.Email, Password = entity.Password };
    }

    public static IEnumerable<User> ToEntities(this IEnumerable<UserModel> models)
    {
        return models.Select(model => model.ToEntity());
    }
}