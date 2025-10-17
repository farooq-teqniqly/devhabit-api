using DevHabit.Api.Entities;
using Microsoft.AspNetCore.Mvc;

namespace DevHabit.Api.Dtos.Habits
{
  public sealed record HabitsQueryParameters
  {
    public bool? IncludeArchived { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;

    [FromQuery(Name = "q")]
    public string? SearchTerm { get; set; }
    public string? Sort { get; set; }
    public HabitType? Type { get; set; }
  }
}
