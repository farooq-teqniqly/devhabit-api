using DevHabit.Api.Entities;

namespace DevHabit.Api.Dtos
{
  public static class HabitMappings
  {
    public static HabitDto ToDto(this Habit habit)
    {
      ArgumentNullException.ThrowIfNull(habit);

      var habitDto = new HabitDto
      {
        Id = habit.Id,
        Name = habit.Name,
        Description = habit.Description,
        Type = (HabitTypeDto)habit.Type,
        Frequency = new FrequencyDto
        {
          Type = (FrequencyTypeDto)habit.Frequency.Type,
          TimesPerPeriod = habit.Frequency.TimesPerPeriod,
        },
        Target = new TargetDto { Value = habit.Target.Value, Unit = habit.Target.Unit },
        Status = (HabitStatusDto)habit.Status,
        IsArchived = habit.IsArchived,
        EndDate = habit.EndDate,
        Milestone =
          habit.Milestone == null
            ? null
            : new MilestoneDto
            {
              Target = habit.Milestone.Target,
              Current = habit.Milestone.Current,
            },
        CreatedAtUtc = habit.CreatedAtUtc,
        UpdatedAtUtc = habit.UpdatedAtUtc,
        LastCompletedAtUtc = habit.LastCompletedAtUtc,
      };

      return habitDto;
    }

    public static Habit ToEntity(this CreateHabitDto dto)
    {
      ArgumentNullException.ThrowIfNull(dto);

      var habit = new Habit
      {
        Id = $"h_{Guid.CreateVersion7()}",
        Name = dto.Name,
        Description = dto.Description,
        Type = (HabitType)dto.Type,
        Frequency = new Frequency
        {
          Type = (FrequencyType)dto.Frequency.Type,
          TimesPerPeriod = dto.Frequency.TimesPerPeriod,
        },
        Target = new Target { Value = dto.Target.Value, Unit = dto.Target.Unit },
        Status = HabitStatus.Ongoing,
        IsArchived = false,
        EndDate = dto.EndDate,
        Milestone = dto.Milestone is not null
          ? new Milestone { Target = dto.Milestone.Target, Current = 0 }
          : null,
        CreatedAtUtc = DateTimeOffset.UtcNow,
      };

      return habit;
    }
  }
}
