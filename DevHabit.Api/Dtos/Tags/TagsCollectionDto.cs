using DevHabit.Api.Dtos.Common;

namespace DevHabit.Api.Dtos.Tags
{
  public sealed record TagsCollectionDto : ICollectionResponse<TagDto>
  {
    public required IReadOnlyCollection<TagDto> Items { get; init; }
  }
}
