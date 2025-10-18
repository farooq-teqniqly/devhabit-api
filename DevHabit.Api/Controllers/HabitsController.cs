using System.Collections.ObjectModel;
using System.Linq.Dynamic.Core;
using System.Net.Mime;
using DevHabit.Api.CustomMediaTypes;
using DevHabit.Api.Database;
using DevHabit.Api.Dtos.Common;
using DevHabit.Api.Dtos.Habits;
using DevHabit.Api.Entities;
using DevHabit.Api.Services.Sorting;
using FluentValidation;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.Internal;

namespace DevHabit.Api.Controllers
{
  [ApiController]
  [Route("habits")]
  [Produces(MediaTypeNames.Application.Json, ApplicationMediaTypes.DevHabitApi)]
  public sealed class HabitsController : ControllerBase
  {
    private readonly ApplicationDbContext _dbContext;

    public HabitsController(ApplicationDbContext dbContext)
    {
      ArgumentNullException.ThrowIfNull(dbContext);

      _dbContext = dbContext;
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
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
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
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
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
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
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PaginationResult<HabitDto>>> GetHabits(
      [FromQuery] HabitsQueryParameters qp,
      SortMappingProvider sortMappingProvider
    )
    {
      ArgumentNullException.ThrowIfNull(sortMappingProvider);

      if (!sortMappingProvider.ValidateMappings<HabitDto, Habit>(qp.Sort))
      {
        return Problem(
          statusCode: StatusCodes.Status400BadRequest,
          detail: $"'{qp.Sort}' is not a valid sort attribute."
        );
      }

      var sortMappings = sortMappingProvider.GetMappings<HabitDto, Habit>();

      var query = _dbContext
        .Habits.Where(h => !h.IsArchived || qp.IncludeArchived == true)
        .Where(h =>
          qp.SearchTerm == null
          || EF.Functions.Like(h.Name, $"%{EscapeLikePattern(qp.SearchTerm)}%")
          || (
            h.Description != null
            && EF.Functions.Like(h.Description, $"%{EscapeLikePattern(qp.SearchTerm)}%")
          )
        )
        .Where(h => qp.Type == null || h.Type == qp.Type)
        .ApplySort(qp.Sort, sortMappings)
        .Select(HabitQueries.ProjectToDto());

      var paginationResult = await PaginationResult<HabitDto>
        .CreateAsync(query, qp.Page, qp.PageSize, HttpContext.RequestAborted)
        .ConfigureAwait(false);

      return Ok(paginationResult);
    }

    [HttpPatch]
    [Route("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
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
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
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

    private static string EscapeLikePattern(string pattern)
    {
      return pattern
        .Replace("[", "[[]", StringComparison.InvariantCultureIgnoreCase)
        .Replace("%", "[%]", StringComparison.InvariantCultureIgnoreCase)
        .Replace("_", "[_]", StringComparison.InvariantCultureIgnoreCase);
    }
  }
}
