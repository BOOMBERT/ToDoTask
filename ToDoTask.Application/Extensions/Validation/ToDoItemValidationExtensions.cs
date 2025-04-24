using FluentValidation;

namespace ToDoTask.Application.Extensions.Validation;

public static class ToDoItemValidationExtensions
{
    public static IRuleBuilderOptions<T, string> IsValidToDoItemTitle<T>(this IRuleBuilder<T, string> ruleBuilder)
    {
        return ruleBuilder
            .MinimumLength(3)
            .WithMessage("Title must be at least 3 characters long.")
            .MaximumLength(128)
            .WithMessage("Title cannot exceed 128 characters.");
    }

    public static IRuleBuilderOptions<T, string> IsValidToDoItemDescription<T>(this IRuleBuilder<T, string> ruleBuilder)
    {
        return ruleBuilder
            .MaximumLength(512)
            .WithMessage("Description cannot exceed 512 characters.");
    }
}
