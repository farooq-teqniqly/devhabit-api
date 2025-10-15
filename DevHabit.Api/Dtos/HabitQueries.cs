using System.Linq.Expressions;
using DevHabit.Api.Entities;

namespace DevHabit.Api.Dtos
{
  public static class HabitQueries
  {
    public static Expression<Func<Habit, HabitDto>> ProjectToDto() => habit => habit.ToDto();
  }
}
