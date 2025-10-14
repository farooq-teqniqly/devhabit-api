using System.Collections.ObjectModel;

namespace DevHabit.Api.Dtos;

public sealed record HabitsCollectionDto
{
  public required ReadOnlyCollection<HabitDto> Items { get; init; }
}

public sealed record HabitDto
{
  public required DateTimeOffset CreatedAtUtc { get; init; }
  public string? Description { get; init; }
  public DateOnly? EndDate { get; init; }
  public required FrequencyDto Frequency { get; init; }
  public required string Id { get; init; }
  public required bool IsArchived { get; init; }
  public DateTimeOffset? LastCompletedAtUtc { get; init; }
  public MilestoneDto? Milestone { get; init; }
  public required string Name { get; init; }
  public required HabitStatusDto Status { get; init; }
  public required TargetDto Target { get; init; }
  public required HabitTypeDto Type { get; init; }
  public DateTimeOffset? UpdatedAtUtc { get; init; }
}
