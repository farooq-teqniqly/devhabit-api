using FluentValidation;

namespace DevHabit.Api.Dtos
{
  public sealed record CreateTagDto
  {
    public string? Description { get; set; }
    public required string Name { get; set; }
  }

  internal sealed class CreateTagDtoValidator : AbstractValidator<CreateTagDto>
  {
    public CreateTagDtoValidator()
    {
      RuleFor(t => t.Name).NotEmpty().MinimumLength(3);
      RuleFor(t => t.Description).MinimumLength(5).MaximumLength(500);
    }
  }
}
