using FluentValidation;

namespace DevHabit.Api.Dtos.Auth
{
  public sealed record LoginUserDto
  {
    public required string Email { get; init; }
    public required string Password { get; init; }
  }

  internal sealed class LoginUserDtoValidator : AbstractValidator<LoginUserDto>
  {
    public LoginUserDtoValidator()
    {
      RuleFor(u => u.Email).NotEmpty().EmailAddress().MinimumLength(5).MaximumLength(100);
      RuleFor(u => u.Password).NotEmpty().MinimumLength(8).MaximumLength(100);
    }
  }
}
