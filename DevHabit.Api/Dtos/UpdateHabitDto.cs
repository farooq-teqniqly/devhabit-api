using DevHabit.Api.Dtos;

namespace DevHabit.Api.Dtos
{
  public sealed record UpdateHabitDto
  {
    public string? Description { get; init; }
    public DateOnly? EndDate { get; init; }
    public required FrequencyDto Frequency { get; init; }
    public UpdateMilestoneDto? Milestone { get; init; }
    public required string Name { get; init; }
    public required TargetDto Target { get; init; }
    public required HabitTypeDto Type { get; init; }
  }
}
