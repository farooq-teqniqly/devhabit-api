namespace DevHabit.Api.Dtos
{
  public sealed record UpdateTagDto
  {
    public string? Description { get; set; }
    public required string Name { get; set; }
  }
}
