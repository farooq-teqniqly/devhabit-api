namespace DevHabit.Api.Services.Sorting
{
  public sealed class SortMappingProvider
  {
    private readonly IEnumerable<ISortMappingDefinition> _sortMappingDefinitions;

    public SortMappingProvider(IEnumerable<ISortMappingDefinition> sortMappingDefinitions)
    {
      ArgumentNullException.ThrowIfNull(sortMappingDefinitions);
      _sortMappingDefinitions = sortMappingDefinitions;
    }

    public IEnumerable<SortMapping> GetMappings<TSource, TTarget>()
    {
      var sortMappingDefinition =
        _sortMappingDefinitions.OfType<SortMappingDefinition<TSource, TTarget>>().FirstOrDefault()
        ?? throw new InvalidOperationException(
          $"The mapping from '{typeof(TSource).Name}' to '{typeof(TTarget).Name}' is undefined."
        );

      return sortMappingDefinition.Mappings;
    }

    public bool ValidateMappings<TSource, TDestination>(string? sort)
    {
      if (string.IsNullOrWhiteSpace(sort))
      {
        return true;
      }

      var sortFields = SortFieldParser.ExtractFieldNames(sort);

      var mappings = GetMappings<TSource, TDestination>();

      return sortFields.All(f =>
        mappings.Any(m => m.SortField.Equals(f, StringComparison.OrdinalIgnoreCase))
      );
    }
  }
}
