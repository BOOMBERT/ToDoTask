using FluentValidation;

namespace ToDoTask.Application.Extensions.Validation;

public static class GeneralValidationExtensions
{
    public static IRuleBuilderOptions<T, DateTime> IsFutureUtc<T>(this IRuleBuilder<T, DateTime> ruleBuilder)
    {
        return ruleBuilder
            .GreaterThan(DateTime.UtcNow)
            .WithMessage("Date time must be in the future.");
    }

    public static IRuleBuilderOptions<T, decimal> IsPercentBetween0And100WithMaxTwoDecimals<T>(this IRuleBuilder<T, decimal> ruleBuilder)
    {
        return ruleBuilder
            .InclusiveBetween(0, 100)
            .WithMessage("Percent must be between 0 and 100.")
            .Must(value => value == Math.Round(value, 2))
            .WithMessage("Percent can have a maximum of two decimal places.");
    }
}
