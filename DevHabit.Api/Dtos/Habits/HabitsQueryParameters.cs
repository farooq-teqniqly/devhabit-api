﻿using DevHabit.Api.Entities;
using Microsoft.AspNetCore.Mvc;

namespace DevHabit.Api.Dtos.Habits
{
  public sealed record HabitsQueryParameters
  {
    public bool? IncludeArchived { get; set; }
    public HabitType? Type { get; set; }

    [FromQuery(Name = "q")]
    public string? SearchTerm { get; set; }
  }
}
