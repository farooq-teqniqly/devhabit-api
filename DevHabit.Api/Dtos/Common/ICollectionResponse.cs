namespace DevHabit.Api.Dtos.Common
{
  public interface ICollectionResponse<T>
  {
    IReadOnlyCollection<T> Items { get; init; }
  }
}
