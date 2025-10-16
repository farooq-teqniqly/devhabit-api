namespace DevHabit.Api.Dtos.Tags
{
  public sealed record UpdateTagDto
  {
    public string? Description { get; set; }
    public required string Name { get; set; }
  }
}
