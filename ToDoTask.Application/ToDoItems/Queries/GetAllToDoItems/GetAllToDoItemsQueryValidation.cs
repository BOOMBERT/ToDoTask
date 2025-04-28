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
    }
}
