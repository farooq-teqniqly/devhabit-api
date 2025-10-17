namespace DevHabit.Api.Services.Sorting
{
#pragma warning disable S2326
  public sealed class SortMappingDefinition<TSource, TTarget> : ISortMappingDefinition
#pragma warning restore S2326
  {
    public required IEnumerable<SortMapping> Mappings { get; init; }
  }
}
