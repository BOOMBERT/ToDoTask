using FluentValidation;
using ToDoTask.Application.Interfaces;
using ToDoTask.Domain.Constants;

namespace ToDoTask.Application.Extensions.Validation;

public static class PaginationAndSortingValidationExtensions
{
    public static IRuleBuilderOptions<T, int> IsValidPageNumber<T>(this IRuleBuilder<T, int> ruleBuilder) 
        where T : IPaginationAndSortingQuery
    {
        return ruleBuilder
            .GreaterThanOrEqualTo(1).WithMessage("Page number must be greater than or equal to 1");
    }

    public static IRuleBuilderOptions<T, int> IsValidPageSize<T>(this IRuleBuilder<T, int> ruleBuilder, int[] allowedPageSizes) 
        where T : IPaginationAndSortingQuery
    {
        return ruleBuilder
            .Must(ps => allowedPageSizes.Contains(ps)).WithMessage($"Page size must be in [{string.Join(",", allowedPageSizes)}]");
    }

    public static IRuleBuilderOptions<T, string?> IsValidSortBy<T>(this IRuleBuilder<T, string?> ruleBuilder, string[] allowedSortByColumnNames) 
        where T : IPaginationAndSortingQuery
    {
        return ruleBuilder
            .Must((instance, sortBy) => 
                (string.IsNullOrEmpty(sortBy) && instance.SortDirection == null) ||
                (!string.IsNullOrWhiteSpace(sortBy) && instance.SortDirection != null &&
                    allowedSortByColumnNames.Any(column => string.Equals(column, sortBy, StringComparison.OrdinalIgnoreCase)))
            )
            .WithMessage($"SortBy must be null if SortDirection is null, or must be one of [{string.Join(", ", allowedSortByColumnNames)}] when SortDirection is specified.");
    }

    public static IRuleBuilderOptions<T, SortDirection?> IsValidSortDirection<T>(this IRuleBuilder<T, SortDirection?> ruleBuilder) 
        where T : IPaginationAndSortingQuery
    {
        return ruleBuilder
            .Must((instance, sortDirection) =>
                (sortDirection == null && string.IsNullOrEmpty(instance.SortBy)) ||
                (sortDirection != null && !string.IsNullOrWhiteSpace(instance.SortBy))
            )
            .WithMessage("SortDirection must be provided if SortBy is provided, or must be null if SortBy is null.");
    }
}
