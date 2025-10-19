namespace DevHabit.Api.Entities
{
  public sealed class User
  {
    public DateTimeOffset CreatedAtUtc { get; set; }
    public required string Email { get; set; }
    public required string Id { get; set; }
    public string? IdentityId { get; set; }
    public required string Name { get; set; }
    public DateTimeOffset? UpdatedAtUtc { get; set; }
  }
}
