using MediatR;
using ToDoTask.Application.ToDoItems.Dtos;

namespace ToDoTask.Application.ToDoItems.Queries.GetToDoItemById;

/// <summary>
/// Query for retrieving a specific ToDo item by its unique identifier.
/// </summary>
/// <param name="id">The unique identifier (Guid) of the ToDo item to be retrieved.</param>
public class GetToDoItemByIdQuery(Guid id) : IRequest<ToDoItemDto>
{
    public Guid Id { get; } = id;
}
