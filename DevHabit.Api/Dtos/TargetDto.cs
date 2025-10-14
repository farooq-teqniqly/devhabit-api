namespace DevHabit.Api.Dtos;

public sealed record TargetDto
{
    public required string Unit { get; init; }
    public required int Value { get; init; }
}