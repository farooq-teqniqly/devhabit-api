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

    [HttpPost]
    public async Task<ActionResult<HabitDto>> CreateHabit(CreateHabitDto createHabitDto)
    {
      var habit = createHabitDto.ToEntity();

      await _dbContext.Habits.AddAsync(habit, HttpContext.RequestAborted).ConfigureAwait(false);
      await _dbContext.SaveChangesAsync(HttpContext.RequestAborted).ConfigureAwait(false);

      var habitDto = habit.ToDto();

      return CreatedAtAction(nameof(GetHabit), new { id = habitDto.Id }, habitDto);
    }

    [HttpDelete]
    [Route("{id}")]
    public async Task<ActionResult> DeleteHabit(string id)
    {
      var habit = await _dbContext
        .Habits.FindAsync(id, HttpContext.RequestAborted)
        .ConfigureAwait(false);

      if (habit is null)
      {
        return NotFound();
      }

      _dbContext.Habits.Remove(habit);
      await _dbContext.SaveChangesAsync(HttpContext.RequestAborted).ConfigureAwait(false);

      return NoContent();
    }

    [HttpGet]
    [Route("{id}")]
    public async Task<ActionResult<HabitDto>> GetHabit(string id)
    {
      var habit = await _dbContext
        .Habits.FindAsync(id, HttpContext.RequestAborted)
        .ConfigureAwait(false);

      if (habit is null)
      {
        return NotFound();
      }

      var habitDto = habit.ToDto();

      return Ok(habitDto);
    }

    [HttpGet]
    public async Task<ActionResult<HabitsCollectionDto>> GetHabits()
    {
      var habitDtos = await _dbContext
        .Habits.Select(h => h.ToDto())
        .ToListAsync(HttpContext.RequestAborted)
        .ConfigureAwait(false);

      var habitCollection = new HabitsCollectionDto
      {
        Items = new ReadOnlyCollection<HabitDto>(habitDtos),
      };

      return Ok(habitCollection);
    }
  }
}
