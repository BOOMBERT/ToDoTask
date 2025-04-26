using FluentValidation;
using ToDoTask.Application.Extensions.Validation;

namespace ToDoTask.Application.ToDoItems.Commands.SetToDoItemCompletionPercentage;

public class SetToDoItemCompletionPercentageCommandValidation : AbstractValidator<SetToDoItemCompletionPercentageCommand>
{
    public SetToDoItemCompletionPercentageCommandValidation()
    {
        RuleFor(toDoItem => toDoItem.CompletionPercentage)
            .IsPercentBetween0And100WithMaxTwoDecimals();
    }
}
