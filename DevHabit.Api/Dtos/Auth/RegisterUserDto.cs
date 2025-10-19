using FluentValidation;

namespace DevHabit.Api.Dtos.Auth
{
  public sealed record RegisterUserDto
  {
    public required string ConfirmPassword { get; init; }
    public required string Email { get; init; }
    public required string Name { get; init; }
    public required string Password { get; init; }
  }

  internal sealed class RegisterUserDtoValidator : AbstractValidator<RegisterUserDto>
  {
    public RegisterUserDtoValidator()
    {
      RuleFor(u => u.Name).NotEmpty().MinimumLength(5).MaximumLength(100);

      RuleFor(u => u.Email).NotEmpty().EmailAddress().MinimumLength(5).MaximumLength(100);

      RuleFor(u => u.Password).NotEmpty().MinimumLength(8).MaximumLength(100);
      RuleFor(u => u.ConfirmPassword).NotEmpty().MinimumLength(8).MaximumLength(100);

      RuleFor(u => u)
        .Must(u => u.Password == u.ConfirmPassword)
        .WithMessage("Password and Confirm Password must match.");
    }
  }
}
