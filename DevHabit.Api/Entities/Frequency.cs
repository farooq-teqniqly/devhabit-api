namespace DevHabit.Api.Entities
{
  internal sealed class Frequency
  {
    public required int TimesPerPeriod { get; set; }
    public required FrequencyType Type { get; set; }
  }
}
