using FluentValidation;
using ToDoTask.Application.Extensions.Validation;
using ToDoTask.Domain.Entities;

namespace ToDoTask.Application.ToDoItems.Queries.GetAllToDoItems;

public class GetAllToDoItemsQueryValidation : AbstractValidator<GetAllToDoItemsQuery>
{
    private readonly int[] allowedPageSizes = { 5, 10, 20, 50 };
    private readonly string[] allowedSortByColumnNames = { 
        nameof(ToDoItem.Title),
        nameof(ToDoItem.Description),
        nameof(ToDoItem.ExpiryDateTimeUtc), 
        nameof(ToDoItem.CompletionPercentage)
    };

    public GetAllToDoItemsQueryValidation()
    {
        RuleFor(query => query.SearchPhrase)
            .MaximumLength(512).WithMessage("Search phrase cannot exceed 512 characters.");

        RuleFor(query => query.PageNumber)
            .IsValidPageNumber();

        RuleFor(query => query.PageSize)
            .IsValidPageSize(allowedPageSizes);

        RuleFor(query => query.SortBy)
            .IsValidSortBy(allowedSortByColumnNames);

        RuleFor(query => query.SortDirection)
            .IsValidSortDirection();

        RuleFor(query => query.DateTimeRangeFilter)
            .Must((instance, dateTimeRangeFilter) =>
                (dateTimeRangeFilter == null && string.IsNullOrEmpty(instance.TimeZoneId)) ||
                (dateTimeRangeFilter != null && !string.IsNullOrWhiteSpace(instance.TimeZoneId))
            )
            .WithMessage("DateTimeRangeFilter must be specified when TimeZoneId is provided, or must be null if TimeZoneId is null.");

        RuleFor(query => query.TimeZoneId)
            .Must((instance, timeZoneId) =>
                (string.IsNullOrEmpty(timeZoneId) && instance.DateTimeRangeFilter == null) ||
                (!string.IsNullOrWhiteSpace(timeZoneId) && instance.DateTimeRangeFilter != null)
            )
            .WithMessage("TimeZoneId must be specified when DateTimeRangeFilter is provided, or must be null if DateTimeRangeFilter is null.");

        RuleFor(query => query.TimeZoneId)
            .Must((instance, timeZoneId) =>
            {
                if (string.IsNullOrEmpty(timeZoneId))
                    return true;

                try
                {
                    TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
                    return true;
                }
                catch (TimeZoneNotFoundException)
                {
                    return false;
                }
            })
            .When(query => query.DateTimeRangeFilter != null)
            .WithMessage("Invalid time zone id (e.g., 'Europe/Warsaw', 'America/New_York').");
    }
}
