using DevHabit.Api.Dtos.Auth;
using DevHabit.Api.Entities;

namespace DevHabit.Api.Dtos.Users
{
  public static class UserMappings
  {
    public static User ToEntity(this RegisterUserDto dto)
    {
      ArgumentNullException.ThrowIfNull(dto);

      var user = new User
      {
        Id = $"u_{Guid.CreateVersion7()}",
        Email = dto.Email,
        Name = dto.Email,
        CreatedAtUtc = DateTimeOffset.UtcNow,
      };

      return user;
    }
  }
}
