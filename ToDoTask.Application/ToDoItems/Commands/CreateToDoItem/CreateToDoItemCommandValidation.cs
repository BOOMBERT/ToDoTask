using FluentValidation;
using ToDoTask.Application.Extensions.Validation;

namespace ToDoTask.Application.ToDoItems.Commands.CreateToDoItem;

public class CreateToDoItemCommandValidation : AbstractValidator<CreateToDoItemCommand>
{
    public CreateToDoItemCommandValidation()
    {
        RuleFor(toDoItem => toDoItem.Title)
            .IsValidToDoItemTitle();

        RuleFor(toDoItem => toDoItem.Description)
            .IsValidToDoItemDescription();

        RuleFor(toDoItem => toDoItem.ExpiryDateTime)
            .IsNotPastOrPresent();

        RuleFor(toDoItem => toDoItem.CompletionPercentage)
            .IsPercentBetween0And100WithMaxTwoDecimals();
    }
}
