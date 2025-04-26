using MediatR;

namespace ToDoTask.Application.ToDoItems.Commands.SetToDoItemCompletionPercentage;

/// <summary>
/// Command for setting the completion percentage of an existing ToDo item.
/// </summary>
/// <remarks>
/// - Id: Required and must be a valid GUID of an existing ToDo item.
/// - CompletionPercentage: Required and must be a decimal between 0 and 100 with up to two decimal places.
/// </remarks>
public class SetToDoItemCompletionPercentageCommand : IRequest
{
    public Guid Id { get; set;  }
    public decimal CompletionPercentage { get; set; }
}
