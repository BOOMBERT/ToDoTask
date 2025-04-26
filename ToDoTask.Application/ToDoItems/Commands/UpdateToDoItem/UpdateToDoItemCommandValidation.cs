using FluentValidation;
using ToDoTask.Application.Extensions.Validation;

namespace ToDoTask.Application.ToDoItems.Commands.UpdateToDoItem;

public class UpdateToDoItemCommandValidation : AbstractValidator<UpdateToDoItemCommand>
{
    public UpdateToDoItemCommandValidation()
    {
        RuleFor(toDoItem => toDoItem.Title)
            .IsValidToDoItemTitle();

        RuleFor(toDoItem => toDoItem.Description)
            .IsValidToDoItemDescription();

        RuleFor(toDoItem => toDoItem.ExpiryDateTimeUtc)
            .IsFutureUtc();

        RuleFor(toDoItem => toDoItem.CompletionPercentage)
            .IsPercentBetween0And100WithMaxTwoDecimals();
    }
}
