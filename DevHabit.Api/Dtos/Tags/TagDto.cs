using System.Collections.ObjectModel;

namespace DevHabit.Api.Dtos.Tags
{
  public sealed record TagsCollectionDto
  {
    public required ReadOnlyCollection<TagDto> Items { get; init; }
  }

  public sealed record TagDto
  {
    public required DateTimeOffset CreatedAtUtc { get; set; }
    public string? Description { get; set; }
    public required string Id { get; set; }
    public required string Name { get; set; }
    public DateTimeOffset? UpdatedAtUtc { get; set; }
  }
}
