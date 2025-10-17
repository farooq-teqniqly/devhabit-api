using Microsoft.EntityFrameworkCore;

namespace DevHabit.Api.Dtos.Common
{
  public sealed record PaginationResult<T> : ICollectionResponse<T>
  {
    public bool HasNextPage => Page < TotalPages;
    public bool HasPreviousPage => Page > 1;
    public IReadOnlyCollection<T> Items { get; init; } = new List<T>();

    public int Page { get; init; }
    public int PageSize { get; init; }
    public int TotalCount { get; init; }

    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);

    public static async Task<PaginationResult<T>> CreateAsync(
      IQueryable<T> query,
      int page,
      int pageSize,
      CancellationToken cancellationToken = default
    )
    {
      var totalCount = await query.CountAsync(cancellationToken).ConfigureAwait(false);

      var items = await query
        .Skip((page - 1) * pageSize)
        .Take(pageSize)
        .ToListAsync(cancellationToken)
        .ConfigureAwait(false);

      return new PaginationResult<T>
      {
        Items = items,
        Page = page,
        PageSize = pageSize,
        TotalCount = totalCount,
      };
    }
  }
}
