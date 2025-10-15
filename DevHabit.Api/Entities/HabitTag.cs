namespace DevHabit.Api.Entities
{
  public sealed class HabitTag
  {
    public required DateTimeOffset CreatedAtUtc { get; set; }
    public required string HabitId { get; set; }
    public required string TagId { get; set; }
  }
}
