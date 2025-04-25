using MediatR;

namespace ToDoTask.Application.ToDoItems.Commands.CreateToDoItem;

/// <summary>
/// Command for creating a new ToDo item.
/// </summary>
/// <remarks>
/// - Title: Required and must be between 3 and 128 characters inclusive.
/// - Description: Not required and cannot exceed 512 characters.
/// - ExpiryDateTime: Required and must be a future date time.
/// - CompletionPercentage: Not required and must be a decimal between 0 and 100 with up to two decimal places.
/// </remarks>
public class CreateToDoItemCommand : IRequest<Guid>
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTimeOffset ExpiryDateTime { get; set; }
    public decimal CompletionPercentage { get; set; } = 0m;
}
