using FluentValidation;

namespace DevHabit.Api.Dtos.Auth
{
  public sealed record TokenRequestDto
  {
    public required string UserId { get; init; }
    public required string Email { get; init; }
  }

  internal sealed class TokenRequestDtoValidator : AbstractValidator<TokenRequestDto>
  {
    public TokenRequestDtoValidator()
    {
      RuleFor(u => u.Email).NotEmpty().EmailAddress().MinimumLength(5).MaximumLength(100);

      RuleFor(u => u.UserId)
        .NotEmpty()
        .Matches("^[0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{12}$")
        .WithMessage("UserId must be a valid valid UUID.");
    }
  }
}
