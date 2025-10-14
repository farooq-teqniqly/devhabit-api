using System.Linq.Expressions;
using DevHabit.Api.Entities;

namespace DevHabit.Api.Dtos
{
  public static class HabitQueries
  {
    public static Expression<Func<Habit, HabitDto>> ProjectToDto()
    {
      return habit => new HabitDto
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
    }
  }
}
