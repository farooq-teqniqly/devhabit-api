using FluentValidation;

namespace DevHabit.Api.Settings
{
  internal sealed class JwtAuthOptionsValidator : AbstractValidator<JwtAuthOptions>
  {
    public JwtAuthOptionsValidator()
    {
      RuleFor(opts => opts.Audience).Equal("devhabit.app");
      RuleFor(opts => opts.Issuer).Equal("devhabit.api");
      RuleFor(opts => opts.Key).NotEmpty().MinimumLength(32);
      RuleFor(opts => opts.ExpirationInMinutes).GreaterThan(0).LessThanOrEqualTo(30);
      RuleFor(opts => opts.RefreshTokenExpirationInDays).GreaterThan(0).LessThanOrEqualTo(7);
    }
  }
}
