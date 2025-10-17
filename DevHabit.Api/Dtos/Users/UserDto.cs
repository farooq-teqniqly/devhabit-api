namespace DevHabit.Api.Dtos.Users
{
  public sealed record UserDto
  {
    public DateTimeOffset CreatedAtUtc { get; set; }
    public required string Email { get; set; }
    public required string Id { get; set; }
    public required string Name { get; set; }
    public DateTimeOffset? UpdatedAtUtc { get; set; }
  }
}
