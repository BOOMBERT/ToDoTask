using MediatR;

namespace ToDoTask.Application.ToDoItems.Commands.DeleteToDoItem;

/// <summary>
/// Command for deleting an existing ToDo item.
/// </summary>
/// <param name="Id">The unique identifier of the ToDo item to delete.</param>
public class DeleteToDoItemCommand(Guid Id) : IRequest
{
    public Guid Id { get; set; } = Id;
}
