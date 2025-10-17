using DevHabit.Api.Dtos.Common;

namespace DevHabit.Api.Dtos.Habits
{
  public sealed record HabitsCollectionDto : ICollectionResponse<HabitDto>
  {
    public required IReadOnlyCollection<HabitDto> Items { get; init; }
  }
}
