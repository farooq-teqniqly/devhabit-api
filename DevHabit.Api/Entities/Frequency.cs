namespace DevHabit.Api.Entities
{
  public sealed class Frequency
  {
    public required int TimesPerPeriod { get; set; }
    public required FrequencyType Type { get; set; }
  }
}
