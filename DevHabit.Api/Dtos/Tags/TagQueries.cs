using System.Linq.Expressions;
using DevHabit.Api.Entities;

namespace DevHabit.Api.Dtos
{
  public static class TagQueries
  {
    public static Expression<Func<Tag, TagDto>> ProjectToDto() => tag => tag.ToDto();
  }
}
