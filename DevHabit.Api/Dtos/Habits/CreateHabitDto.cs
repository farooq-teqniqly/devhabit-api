using FluentValidation;

namespace DevHabit.Api.Dtos.Habits
{
  public sealed record CreateHabitDto
  {
    public string? Description { get; init; }
    public DateOnly? EndDate { get; init; }
    public required FrequencyDto Frequency { get; init; }
    public MilestoneDto? Milestone { get; init; }
    public required string Name { get; init; }
    public required TargetDto Target { get; init; }
    public required HabitTypeDto Type { get; init; }
  }

  internal sealed class CreateHabitDtoValidator : AbstractValidator<CreateHabitDto>
  {
    private static readonly string[] _allowedUnits =
    [
      "MINUTES",
      "HOURS",
      "STEPS",
      "KM",
      "CAL",
      "PAGES",
      "BOOKS",
      "TASKS",
      "SESSIONS",
    ];

    private static readonly string[] _allowedUnitsForBinaryHabits = ["SESSIONS", "TASKS"];

    public CreateHabitDtoValidator()
    {
      RuleFor(h => h.Name)
        .NotEmpty()
        .MinimumLength(3)
        .MaximumLength(100)
        .WithMessage("Habit name must be between 3 and 100 characters.");

      RuleFor(h => h.Description)
        .MinimumLength(5)
        .MaximumLength(500)
        .When(h => h.Description is not null)
        .WithMessage("Description must be between 5 and 500 characters.");

      RuleFor(h => h.Type).IsInEnum().WithMessage("Invalid habit type.");

      RuleFor(h => h.Frequency.Type).IsInEnum().WithMessage("Invalid frequency period.");

      RuleFor(h => h.Frequency.TimesPerPeriod)
        .GreaterThan(0)
        .WithMessage("Frequency must be greater than 0.");

      RuleFor(h => h.Target.Value)
        .GreaterThan(0)
        .WithMessage("Target value must be greater than 0.");

      RuleFor(h => h.Target.Unit)
        .NotEmpty()
        .Must(unit => _allowedUnits.Contains(unit.ToUpperInvariant()))
        .WithMessage($"Unit must be one of: {string.Join(", ", _allowedUnits)}");

      RuleFor(h => h.EndDate)
        .Must(date => date is null || date.Value > DateOnly.FromDateTime(DateTime.UtcNow))
        .WithMessage("End date must be in the future.");

      When(
        h => h.Milestone is not null,
        () =>
        {
          RuleFor(h => h.Milestone!.Target)
            .GreaterThan(0)
            .WithMessage("Milestone target must be greater than 0.");
        }
      );

      RuleFor(h => h.Target.Unit)
        .Must((dto, unit) => IsTargetUnitCompatibleWithType(dto.Type, unit))
        .WithMessage("Target unit is not compatible with the habit type.");
    }

    private static bool IsTargetUnitCompatibleWithType(HabitTypeDto type, string unit)
    {
      var normalizedUnit = unit.ToUpperInvariant();

      return type switch
      {
        HabitTypeDto.Binary => _allowedUnitsForBinaryHabits.Contains(normalizedUnit),
        HabitTypeDto.Measurable => _allowedUnits.Contains(normalizedUnit),
        _ => false,
      };
    }
  }
}
