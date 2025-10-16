using DevHabit.Api.Entities;
using Microsoft.AspNetCore.Mvc;

namespace DevHabit.Api.Dtos.Habits
{
  public sealed record HabitsQueryParameters
  {
    public HabitType? Type { get; init; }

    [FromQuery(Name = "q")]
    public string? SearchTerm { get; set; }
  }
}
