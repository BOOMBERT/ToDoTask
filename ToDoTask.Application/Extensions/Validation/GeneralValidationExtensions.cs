using FluentValidation;

namespace ToDoTask.Application.Extensions.Validation;

public static class GeneralValidationExtensions
{
    public static IRuleBuilderOptions<T, DateTimeOffset> IsNotPastOrPresent<T>(this IRuleBuilder<T, DateTimeOffset> ruleBuilder)
    {
        return ruleBuilder
            .GreaterThanOrEqualTo(DateTimeOffset.Now)
            .WithMessage("Date time cannot be in the past or present.");
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
