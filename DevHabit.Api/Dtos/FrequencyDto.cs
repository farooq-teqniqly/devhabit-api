using DevHabit.Api.Entities;

namespace DevHabit.Api.Dtos;

public sealed record FrequencyDto
{
  public required int TimesPerPeriod { get; init; }
  public required FrequencyType Type { get; init; }
}
