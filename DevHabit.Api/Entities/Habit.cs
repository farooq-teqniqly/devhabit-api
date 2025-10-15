using System.Collections.ObjectModel;

namespace DevHabit.Api.Entities
{
  public sealed class Habit
  {
    public required DateTimeOffset CreatedAtUtc { get; set; }
    public string? Description { get; set; }
    public DateOnly? EndDate { get; set; }
    public required Frequency Frequency { get; set; }
    public List<HabitTag> HabitTags { get; set; } = [];
    public required string Id { get; set; }
    public required bool IsArchived { get; set; }
    public DateTimeOffset? LastCompletedAtUtc { get; set; }
    public Milestone? Milestone { get; set; }
    public required string Name { get; set; }
    public required HabitStatus Status { get; set; }
    public List<Tag> Tags { get; set; } = [];
    public required Target Target { get; set; }
    public required HabitType Type { get; set; }
    public DateTimeOffset? UpdatedAtUtc { get; set; }
  }
}
