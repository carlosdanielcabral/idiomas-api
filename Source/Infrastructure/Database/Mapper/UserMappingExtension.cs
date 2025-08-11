using IdiomasAPI.Source.Domain.Entity;
using IdiomasAPI.Source.Infrastructure.Database.Model;

namespace IdiomasAPI.Source.Infrastructure.Database.Mapper;

public static class UserMappingExtension
{
    public static User ToEntity(this UserModel model)
    {
        return new User(model.Id, model.Name, model.Email, model.Password);
    }

    public static UserModel ToModel(this User entity)
    {
        return new UserModel() { Id = entity.Id, Name = entity.Name, Email = entity.Email, Password = entity.Password };
    }

    public static IEnumerable<User> ToEntities(this IEnumerable<UserModel> models)
    {
        return models.Select(model => model.ToEntity());
    }
}