using DevHabit.Api.Database;
using DevHabit.Api.Dtos.HabitTags;
using DevHabit.Api.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DevHabit.Api.Controllers
{
  [ApiController]
  [Route("habits/{habitId}/tags")]
  [Authorize]
  public sealed class HabitTagsController : ControllerBase
  {
    private readonly ApplicationDbContext _dbContext;

    public HabitTagsController(ApplicationDbContext dbContext)
    {
      ArgumentNullException.ThrowIfNull(dbContext);

      _dbContext = dbContext;
    }

    [HttpDelete]
    [Route("{tagId}")]
    public async Task<ActionResult> RemoveTag(string habitId, string tagId)
    {
      var habitTag = await _dbContext
        .HabitTags.SingleOrDefaultAsync(
          ht => ht.HabitId == habitId && ht.TagId == tagId,
          HttpContext.RequestAborted
        )
        .ConfigureAwait(false);

      if (habitTag == null)
      {
        return NotFound();
      }

      _dbContext.HabitTags.Remove(habitTag);

      await _dbContext.SaveChangesAsync(HttpContext.RequestAborted).ConfigureAwait(false);

      return NoContent();
    }

    [HttpPut]
    public async Task<ActionResult> UpsertTags(
      string habitId,
      UpsertHabitTagsDto upsertHabitTagsDto
    )
    {
      var habit = await _dbContext
        .Habits.Include(h => h.HabitTags)
        .FirstOrDefaultAsync(h => h.Id == habitId, HttpContext.RequestAborted)
        .ConfigureAwait(false);

      if (habit == null)
      {
        return NotFound();
      }

      var currentTagIds = habit.HabitTags.Select(ht => ht.TagId).ToHashSet();

#pragma warning disable CA1062
      if (currentTagIds.SetEquals(upsertHabitTagsDto.TagIds))
#pragma warning restore CA1062
      {
        return NoContent();
      }

      var existingTagIds = await _dbContext
        .Tags.Where(t => upsertHabitTagsDto.TagIds.Contains(t.Id))
        .Select(t => t.Id)
        .ToListAsync(HttpContext.RequestAborted)
        .ConfigureAwait(false);

      if (existingTagIds.Count != upsertHabitTagsDto.TagIds.Count)
      {
        return BadRequest("One or more tag ids are invalid.");
      }

      ((List<HabitTag>)habit.HabitTags).RemoveAll(ht =>
        !upsertHabitTagsDto.TagIds.Contains(ht.TagId)
      );

      var tagIdsToAdd = upsertHabitTagsDto.TagIds.Except(currentTagIds).ToArray();

      ((List<HabitTag>)habit.HabitTags).AddRange(
        tagIdsToAdd.Select(tagId => new HabitTag
        {
          HabitId = habitId,
          TagId = tagId,
          CreatedAtUtc = DateTimeOffset.UtcNow,
        })
      );

      await _dbContext.SaveChangesAsync(HttpContext.RequestAborted).ConfigureAwait(false);

      return NoContent();
    }
  }
}
