using MediatR;
using Microsoft.AspNetCore.Mvc;
using ToDoTask.Application.ToDoItems.Commands;

namespace ToDoTask.API.Controllers;

[Route("api/todoitems")]
[ApiController]
public class ToDoItemsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ToDoItemsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Creates a new ToDo item.
    /// </summary>
    /// <param name="command">Details of the ToDo item to create.</param>
    /// <returns>The unique identifier (Guid) of the newly created ToDo item.</returns>
    /// <response code="201">The ToDo item was successfully created.</response>
    /// <response code="400">The request is invalid (e.g., validation errors).</response>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateToDoItem([FromBody] CreateToDoItemCommand command)
    {
        var toDoItemId = await _mediator.Send(command);
        return Created(string.Empty, toDoItemId);
    }
}
