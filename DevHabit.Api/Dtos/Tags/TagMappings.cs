using DevHabit.Api.Entities;

namespace DevHabit.Api.Dtos
{
  public static class TagMappings
  {
    public static TagDto ToDto(this Tag tag)
    {
      ArgumentNullException.ThrowIfNull(tag);

      var tagDto = new TagDto
      {
        Id = tag.Id,
        Name = tag.Name,
        Description = tag.Description,
        CreatedAtUtc = tag.CreatedAtUtc,
        UpdatedAtUtc = tag.UpdatedAtUtc,
      };

      return tagDto;
    }

    public static Tag ToEntity(this CreateTagDto dto)
    {
      ArgumentNullException.ThrowIfNull(dto);

      var tag = new Tag
      {
        Id = $"t_{Guid.CreateVersion7()}",
        Name = dto.Name,
        Description = dto.Description,
        CreatedAtUtc = DateTimeOffset.UtcNow,
      };

      return tag;
    }

    public static void UpdateFromDto(this Tag tag, UpdateTagDto dto)
    {
      ArgumentNullException.ThrowIfNull(tag);
      ArgumentNullException.ThrowIfNull(dto);

      tag.Name = dto.Name;
      tag.Description = dto.Description;
      tag.UpdatedAtUtc = DateTimeOffset.UtcNow;
    }
  }
}
