using System.Collections.ObjectModel;

namespace DevHabit.Api.Dtos.HabitTags
{
  public sealed record UpsertHabitTagsDto
  {
    public required ReadOnlyCollection<string> TagIds { get; init; }
  }
}
