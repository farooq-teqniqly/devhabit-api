using DevHabit.Api.Entities;
using DevHabit.Api.Services.Sorting;

namespace DevHabit.Api.Dtos.Habits
{
  public static class HabitMappings
  {
    public static readonly SortMappingDefinition<HabitDto, Habit> SortMapping = new()
    {
      Mappings =
      [
        new SortMapping(nameof(HabitDto.Name), nameof(Habit.Name)),
        new SortMapping(nameof(HabitDto.Type), nameof(Habit.Type)),
        new SortMapping(nameof(HabitDto.Status), nameof(Habit.Status)),
        new SortMapping(nameof(HabitDto.EndDate), nameof(Habit.EndDate)),
        new SortMapping(nameof(HabitDto.UpdatedAtUtc), nameof(Habit.UpdatedAtUtc)),
        new SortMapping(nameof(HabitDto.LastCompletedAtUtc), nameof(Habit.LastCompletedAtUtc)),
        new SortMapping(
          $"{nameof(HabitDto.Frequency)}.{nameof(FrequencyDto.Type)}",
          $"{nameof(Habit.Frequency)}.{nameof(Frequency.Type)}"
        ),
      ],
    };

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

    public static HabitWithTagsDto ToDtoWithTags(this Habit habit)
    {
      ArgumentNullException.ThrowIfNull(habit);

      var habitDto = new HabitWithTagsDto
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
        Tags = habit.Tags.Select(t => t.Name).ToArray(),
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

    public static void UpdateFromDto(this Habit habit, UpdateHabitDto dto)
    {
      ArgumentNullException.ThrowIfNull(habit);
      ArgumentNullException.ThrowIfNull(dto);

      habit.Name = dto.Name;
      habit.Description = dto.Description;
      habit.Type = (HabitType)dto.Type;
      habit.EndDate = dto.EndDate;
      habit.Status = dto.Status == null ? HabitStatus.Ongoing : (HabitStatus)dto.Status;

      habit.Frequency = new Frequency
      {
        Type = (FrequencyType)dto.Frequency.Type,
        TimesPerPeriod = dto.Frequency.TimesPerPeriod,
      };

      habit.Target = new Target { Value = dto.Target.Value, Unit = dto.Target.Unit };

      if (dto.Milestone is not null)
      {
        habit.Milestone = new Milestone
        {
          Target = dto.Milestone.Target,
          Current = dto.Milestone.Current,
        };
      }
      else
      {
        habit.Milestone = null;
      }

      habit.UpdatedAtUtc = DateTimeOffset.UtcNow;
    }
  }
}
