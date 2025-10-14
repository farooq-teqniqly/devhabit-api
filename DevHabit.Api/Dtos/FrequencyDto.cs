namespace DevHabit.Api.Dtos;

public sealed record FrequencyDto
{
  public required int TimesPerPeriod { get; init; }
  public required FrequencyTypeDto Type { get; init; }
}
