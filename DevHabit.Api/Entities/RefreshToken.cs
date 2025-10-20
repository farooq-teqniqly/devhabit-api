using Microsoft.AspNetCore.Identity;

namespace DevHabit.Api.Entities
{
  public sealed class RefreshToken
  {
    public required DateTimeOffset ExpiresAtUtc { get; set; }
    public Guid Id { get; set; }
    public required string Token { get; set; }
    public IdentityUser User { get; set; } = null!;
    public required string UserId { get; set; }
  }
}
