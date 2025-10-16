using System.Collections.ObjectModel;
using DevHabit.Api.Database;
using DevHabit.Api.Dtos.Habits;
using DevHabit.Api.Entities;
using FluentValidation;
using Microsoft.AspNetCore.JsonPatch;
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
    public async Task<ActionResult<HabitDto>> CreateHabit(
      [FromBody] CreateHabitDto createHabitDto,
      IValidator<CreateHabitDto> validator
    )
    {
      ArgumentNullException.ThrowIfNull(validator);

      await validator
        .ValidateAndThrowAsync(createHabitDto, HttpContext.RequestAborted)
        .ConfigureAwait(false);

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

      habit.IsArchived = true;
      habit.UpdatedAtUtc = DateTimeOffset.UtcNow;
      await _dbContext.SaveChangesAsync(HttpContext.RequestAborted).ConfigureAwait(false);

      return NoContent();
    }

    [HttpGet]
    [Route("{id}")]
    public async Task<ActionResult<HabitWithTagsDto>> GetHabit(
      string id,
      [FromQuery] bool includeArchived = false
    )
    {
      var habitDto = await _dbContext
        .Habits.Include(h => h.Tags)
        .Where(h => h.Id == id)
        .Where(h => !h.IsArchived || includeArchived)
        .Select(HabitQueries.ProjectToDtoWithTags())
        .SingleOrDefaultAsync(HttpContext.RequestAborted)
        .ConfigureAwait(false);

      if (habitDto is null)
      {
        return NotFound();
      }
      return Ok(habitDto);
    }

    [HttpGet]
    public async Task<ActionResult<HabitsCollectionDto>> GetHabits(
      [FromQuery] HabitsQueryParameters qp,
      [FromQuery] bool includeArchived = false
    )
    {
      var habitDtos = await _dbContext
        .Habits.Where(h => !h.IsArchived || includeArchived)
        .Where(h =>
          qp.SearchTerm == null
          || h.Name.Contains(qp.SearchTerm)
          || (h.Description != null && h.Description.Contains(qp.SearchTerm))
        )
        .Where(h => qp.Type == null || h.Type == qp.Type)
        .Select(HabitQueries.ProjectToDto())
        .ToListAsync(HttpContext.RequestAborted)
        .ConfigureAwait(false);

      var habitCollection = new HabitsCollectionDto
      {
        Items = new ReadOnlyCollection<HabitDto>(habitDtos),
      };

      return Ok(habitCollection);
    }

    [HttpPatch]
    [Route("{id}")]
    public async Task<ActionResult> PatchHabit(
      string id,
      [FromBody] JsonPatchDocument<HabitDto> patchDocument
    )
    {
      ArgumentNullException.ThrowIfNull(patchDocument);

      var allowedPaths = new[] { "/name", "/description", "/isarchived" };

      var invalidOperations = patchDocument
        .Operations.Where(op =>
          !allowedPaths.Contains(op.path, StringComparer.InvariantCultureIgnoreCase)
        )
        .ToList();

      if (invalidOperations.Any())
      {
        return ValidationProblem($"Only {string.Join(", ", allowedPaths)} can be patched.");
      }

      var habit = await _dbContext
        .Habits.FindAsync(id, HttpContext.RequestAborted)
        .ConfigureAwait(false);

      if (habit is null)
      {
        return NotFound();
      }

      var habitDto = habit.ToDto();

      patchDocument.ApplyTo(habitDto, ModelState);

      if (!TryValidateModel(habitDto))
      {
        return ValidationProblem(ModelState);
      }

      habit.Name = habitDto.Name;
      habit.Description = habitDto.Description;
      habit.IsArchived = habitDto.IsArchived;
      habit.UpdatedAtUtc = DateTimeOffset.UtcNow;

      await _dbContext.SaveChangesAsync(HttpContext.RequestAborted).ConfigureAwait(false);

      return NoContent();
    }

    [HttpPut]
    [Route("{id}")]
    public async Task<ActionResult> UpdateHabit(string id, [FromBody] UpdateHabitDto updateHabitDto)
    {
      var habit = await _dbContext
        .Habits.FindAsync(id, HttpContext.RequestAborted)
        .ConfigureAwait(false);

      if (habit is null)
      {
        return NotFound();
      }

      habit.UpdateFromDto(updateHabitDto);
      await _dbContext.SaveChangesAsync(HttpContext.RequestAborted).ConfigureAwait(false);

      return NoContent();
    }
  }
}
