namespace DevHabit.Api.Dtos.Habits
{
  public sealed record UpdateMilestoneDto
  {
    public required int Current { get; init; }
    public required int Target { get; init; }
  }
}
