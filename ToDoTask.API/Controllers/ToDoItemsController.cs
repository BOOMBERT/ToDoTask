using MediatR;
using Microsoft.AspNetCore.Mvc;
using ToDoTask.Application.ToDoItems.Dtos;
using ToDoTask.Application.ToDoItems.Commands.CreateToDoItem;
using ToDoTask.Application.ToDoItems.Queries.GetToDoItemById;
using ToDoTask.Application.ToDoItems.Commands.UpdateToDoItem;
using ToDoTask.Application.ToDoItems.Commands.DeleteToDoItem;

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
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
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
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ToDoItemDto>> GetToDoItemById([FromRoute] Guid id)
    {
        var toDoItem = await _mediator.Send(new GetToDoItemByIdQuery(id));
        return Ok(toDoItem);
    }

    /// <summary>
    /// Updates an existing ToDo item with the specified identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the ToDo item to be updated.</param>
    /// <param name="command">The updated values for the ToDo item.</param>
    /// <response code="204">The ToDo item was successfully updated.</response>
    /// <response code="400">The request is invalid (e.g., validation errors).</response>
    /// <response code="404">No ToDo item was found with the specified ID.</response>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateToDoItem([FromRoute] Guid id, [FromBody] UpdateToDoItemCommand command)
    {
        command.Id = id;
        await _mediator.Send(command);
        return NoContent();
    }

    /// <summary>
    /// Deletes an existing ToDo item with the specified identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the ToDo item to be deleted.</param>
    /// <response code="204">The ToDo item was successfully deleted.</response>
    /// <response code="404">No ToDo item was found with the specified ID.</response>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteToDoItem([FromRoute] Guid id)
    {
        var command = new DeleteToDoItemCommand(id);
        await _mediator.Send(command);
        return NoContent();
    }
}
