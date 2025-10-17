using System.Linq.Dynamic.Core;

namespace DevHabit.Api.Services.Sorting
{
  internal static class QueryableExtensions
  {
    public static IQueryable<T> ApplySort<T>(
      this IQueryable<T> query,
      string? sort,
      IEnumerable<SortMapping> mappings,
      string defaultSort = "Name"
    )
    {
      ArgumentNullException.ThrowIfNull(mappings);

      if (string.IsNullOrWhiteSpace(sort))
      {
        var defaultMapping =
          mappings.FirstOrDefault(m =>
            m.SortField.Equals(defaultSort, StringComparison.OrdinalIgnoreCase)
          )
          ?? throw new InvalidOperationException(
            $"Invalid default sort parameter: '{defaultSort}'"
          );

        return query.OrderBy(defaultMapping.PropertyName);
      }

      var sortFields = SortFieldParser.ExtractFieldNames(sort);

      var orderByParts = new List<string>();

      foreach (var field in sortFields)
      {
        var (sortField, isDescending) = ParseSortField(field);

        var mapping =
          mappings.FirstOrDefault(m =>
            m.SortField.Equals(sortField, StringComparison.OrdinalIgnoreCase)
          ) ?? throw new InvalidOperationException($"Invalid sort parameter: '{sortField}'");

        var direction = (isDescending, mapping.Reverse) switch
        {
          (false, false) => "ASC",
          (false, true) => "DESC",
          (true, true) => "ASC",
          (true, false) => "DESC",
        };

        orderByParts.Add($"{mapping.PropertyName} {direction}");
      }

      var orderBy = string.Join(",", orderByParts);

      return query.OrderBy(orderBy);
    }

    private static (string SortField, bool isDescending) ParseSortField(string field)
    {
      var parts = field.Split(" ", StringSplitOptions.RemoveEmptyEntries);
      var sortField = parts[0];

      if (
        parts.Length > 1
        && !parts[1].Equals("desc", StringComparison.OrdinalIgnoreCase)
        && !parts[1].Equals("asc", StringComparison.OrdinalIgnoreCase)
      )
      {
        throw new InvalidOperationException(
          $"Invalid sort direction: '{parts[1]}'. Use 'asc' or 'desc'."
        );
      }

      var isDescending =
        parts.Length > 1 && parts[1].Equals("desc", StringComparison.OrdinalIgnoreCase);

      return (sortField, isDescending);
    }
  }
}
