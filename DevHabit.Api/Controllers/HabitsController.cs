using DevHabit.Api.Database;
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
    public async Task<IActionResult> GetHabits()
    {
      var habits = await _dbContext
        .Habits.ToListAsync(HttpContext.RequestAborted)
        .ConfigureAwait(false);
      return Ok(habits);
    }
  }
}
