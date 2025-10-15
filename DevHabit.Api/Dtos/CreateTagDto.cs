namespace DevHabit.Api.Dtos
{
  public sealed record CreateTagDto
  {
    public string? Description { get; set; }
    public required string Name { get; set; }
  }
}
