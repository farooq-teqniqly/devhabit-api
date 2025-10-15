using System.Collections.ObjectModel;
using DevHabit.Api.Database;
using DevHabit.Api.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DevHabit.Api.Controllers
{
  [ApiController]
  [Route("tags")]
  public class TagsController : ControllerBase
  {
    private readonly ApplicationDbContext _dbContext;

    public TagsController(ApplicationDbContext dbContext)
    {
      ArgumentNullException.ThrowIfNull(dbContext);
      _dbContext = dbContext;
    }

    [HttpPost]
    public async Task<ActionResult<TagDto>> CreateTag([FromBody] CreateTagDto createTagDto)
    {
      var tag = createTagDto.ToEntity();

      var tegExists = await _dbContext
        .Tags.AnyAsync(t => t.Name == tag.Name, HttpContext.RequestAborted)
        .ConfigureAwait(false);

      if (tegExists)
      {
        return Conflict(
          $"The tag with name '{tag.Name}' already exists. Tag names must be globally unique."
        );
      }

      await _dbContext.Tags.AddAsync(tag, HttpContext.RequestAborted).ConfigureAwait(false);
      await _dbContext.SaveChangesAsync(HttpContext.RequestAborted).ConfigureAwait(false);

      var tagDto = tag.ToDto();

      return CreatedAtAction(nameof(GetTags), new { id = tagDto.Id }, tagDto);
    }

    [HttpDelete]
    [Route("{id}")]
    public async Task<ActionResult> DeleteTag(string id)
    {
      var tag = await _dbContext
        .Tags.FindAsync(id, HttpContext.RequestAborted)
        .ConfigureAwait(false);

      if (tag is null)
      {
        return NotFound();
      }

      _dbContext.Tags.Remove(tag);
      await _dbContext.SaveChangesAsync(HttpContext.RequestAborted).ConfigureAwait(false);

      return NoContent();
    }

    [HttpGet]
    [Route("{id}")]
    public async Task<ActionResult<TagDto>> GetTag(string id)
    {
      var tagDto = await _dbContext
        .Tags.Where(h => h.Id == id)
        .Select(TagQueries.ProjectToDto())
        .SingleOrDefaultAsync(HttpContext.RequestAborted)
        .ConfigureAwait(false);

      if (tagDto == null)
      {
        return NotFound();
      }

      return Ok(tagDto);
    }

    [HttpGet]
    public async Task<ActionResult<TagsCollectionDto>> GetTags()
    {
      var tagDtos = await _dbContext
        .Tags.Select(TagQueries.ProjectToDto())
        .ToListAsync(HttpContext.RequestAborted)
        .ConfigureAwait(false);

      var tagsCollection = new TagsCollectionDto
      {
        Items = new ReadOnlyCollection<TagDto>(tagDtos),
      };

      return Ok(tagsCollection);
    }

    [HttpPut]
    [Route("{id}")]
    public async Task<ActionResult> UpdateTag(string id, [FromBody] UpdateTagDto updateTagDto)
    {
      var tag = await _dbContext
        .Tags.FindAsync(id, HttpContext.RequestAborted)
        .ConfigureAwait(false);

      if (tag is null)
      {
        return NotFound();
      }

      tag.UpdateFromDto(updateTagDto);
      await _dbContext.SaveChangesAsync(HttpContext.RequestAborted).ConfigureAwait(false);

      return NoContent();
    }
  }
}
