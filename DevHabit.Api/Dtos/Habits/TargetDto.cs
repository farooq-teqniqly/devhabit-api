namespace DevHabit.Api.Dtos.Habits;

public sealed record TargetDto
{
  public required string Unit { get; init; }
  public required int Value { get; init; }
}
