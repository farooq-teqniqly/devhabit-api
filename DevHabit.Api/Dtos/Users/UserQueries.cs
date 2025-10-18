using System.Linq.Expressions;

namespace DevHabit.Api.Dtos.Users
{
  public static class UserQueries
  {
    public static Expression<Func<Entities.User, UserDto>> ProjectToDto()
    {
      return user => new UserDto
      {
        Id = user.Id,
        Name = user.Name,
        CreatedAtUtc = user.CreatedAtUtc,
        Email = user.Email,
        UpdatedAtUtc = user.UpdatedAtUtc,
      };
    }
  }
}
