namespace DevHabit.Api.Services.Sorting
{
  internal static class SortFieldParser
  {
    public static IEnumerable<string> ExtractFieldNames(string? sort)
    {
      if (string.IsNullOrWhiteSpace(sort))
      {
        return [];
      }

      return sort.Split(",")
        .Select(f => f.Trim().Split(" ", StringSplitOptions.RemoveEmptyEntries)[0])
        .Where(f => !string.IsNullOrWhiteSpace(f));
    }
  }
}
