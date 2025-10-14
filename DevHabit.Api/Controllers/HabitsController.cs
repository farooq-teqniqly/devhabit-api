using System.Collections.ObjectModel;
using DevHabit.Api.Database;
using DevHabit.Api.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DevHabit.Api.Controllers
{
  [ApiController]
  [Route("habits")]
  public sealed class HabitsController : ControllerBase
  {
    private readonly ApplicationDbContext _dbContext;

    public HabitsController(ApplicationDbContext dbContext)
    {
      ArgumentNullException.ThrowIfNull(dbContext);

      _dbContext = dbContext;
    }

    [HttpGet]
    public async Task<ActionResult<HabitsCollectionDto>> GetHabits()
    {
      var habits = await _dbContext
        .Habits.Select(h => new HabitDto
        {
          Id = h.Id,
          Name = h.Name,
          Description = h.Description,
          Type = (HabitTypeDto)h.Type,
          Frequency = new FrequencyDto
          {
            Type = (FrequencyTypeDto)h.Frequency.Type,
            TimesPerPeriod = h.Frequency.TimesPerPeriod,
          },
          Target = new TargetDto { Value = h.Target.Value, Unit = h.Target.Unit },
          Status = (HabitStatusDto)h.Status,
          IsArchived = h.IsArchived,
          EndDate = h.EndDate,
          Milestone =
            h.Milestone == null
              ? null
              : new MilestoneDto { Target = h.Milestone.Target, Current = h.Milestone.Current },
          CreatedAtUtc = h.CreatedAtUtc,
          UpdatedAtUtc = h.UpdatedAtUtc,
          LastCompletedAtUtc = h.LastCompletedAtUtc,
        })
        .ToListAsync(HttpContext.RequestAborted)
        .ConfigureAwait(false);

      var habitCollection = new HabitsCollectionDto
      {
        Items = new ReadOnlyCollection<HabitDto>(habits),
      };

      return Ok(habitCollection);
    }
  }
}
