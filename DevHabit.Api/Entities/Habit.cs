namespace DevHabit.Api.Entities
{
  internal sealed class Habit
  {
    public required DateTime CreatedAtUtc { get; set; }
    public string? Description { get; set; }
    public DateOnly? EndDate { get; set; }
    public required Frequency Frequency { get; set; }
    public required HabitStatus HabitStatus { get; set; }
    public required HabitType HabitType { get; set; }
    public required string Id { get; set; }
    public required bool IsArchived { get; set; }
    public DateTime? LastCompletedAtUtc { get; set; }
    public Milestone? Milestone { get; set; }
    public required string Name { get; set; }
    public required Target Target { get; set; }
    public DateTime? UpdatedAtUtc { get; set; }
  }
}
