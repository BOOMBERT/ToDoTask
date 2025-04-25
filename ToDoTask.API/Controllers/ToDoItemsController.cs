using MediatR;
using Microsoft.AspNetCore.Mvc;
using ToDoTask.Application.ToDoItems.Dtos;
using ToDoTask.Application.ToDoItems.Commands.CreateToDoItem;
using ToDoTask.Application.ToDoItems.Queries.GetToDoItemById;

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
    /// <returns>The location of the newly created ToDo item, provided in the Location header.</returns>
    /// <response code="201">The ToDo item was successfully created.</response>
    /// <response code="400">The request is invalid (e.g., validation errors).</response>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesErrorResponseType(typeof(ValidationProblemDetails))]
    public async Task<IActionResult> CreateToDoItem([FromBody] CreateToDoItemCommand command)
    {
        var toDoItemId = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetToDoItemById), new { id = toDoItemId }, null);
    }

    /// <summary>
    /// Retrieves a specific ToDo item by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the ToDo item to be retrieved.</param>
    /// <returns>The requested ToDo item.</returns>
    /// <response code="200">The ToDo item was successfully found and returned.</response>
    /// <response code="404">No ToDo item was found with the specified ID.</response>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesErrorResponseType(typeof(ProblemDetails))]
    public async Task<ActionResult<ToDoItemDto>> GetToDoItemById([FromRoute] Guid id)
    {
        var toDoItem = await _mediator.Send(new GetToDoItemByIdQuery(id));
        return Ok(toDoItem);
    }
}
