using DevHabit.Api.Database;
using DevHabit.Api.Dtos.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DevHabit.Api.Controllers
{
  [ApiController]
  [Route("users")]
  [Authorize]
  public sealed class UsersController : ControllerBase
  {
    private readonly ApplicationDbContext _dbContext;

    public UsersController(ApplicationDbContext dbContext)
    {
      ArgumentNullException.ThrowIfNull(dbContext);

      _dbContext = dbContext;
    }

    [HttpGet]
    [Route("{id}")]
    public async Task<ActionResult<UserDto>> GetUserById(string id)
    {
      var userDto = await _dbContext
        .Users.Where(u => u.Id == id)
        .Select(UserQueries.ProjectToDto())
        .SingleOrDefaultAsync(HttpContext.RequestAborted)
        .ConfigureAwait(false);

      if (userDto is null)
      {
        return NotFound();
      }

      return Ok(userDto);
    }
  }
}
