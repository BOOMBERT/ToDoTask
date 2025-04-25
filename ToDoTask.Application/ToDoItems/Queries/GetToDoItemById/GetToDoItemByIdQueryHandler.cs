using MediatR;
using ToDoTask.Application.ToDoItems.Dtos;
using ToDoTask.Domain.Entities;
using ToDoTask.Domain.Exceptions;
using ToDoTask.Domain.Repositories;

namespace ToDoTask.Application.ToDoItems.Queries.GetToDoItemById;

public class GetToDoItemByIdQueryHandler : IRequestHandler<GetToDoItemByIdQuery, ToDoItemDto>
{
    private readonly IGenericRepository _genericRepository;

    public GetToDoItemByIdQueryHandler(IGenericRepository genericRepository)
    {
        _genericRepository = genericRepository;
    }

    public async Task<ToDoItemDto> Handle(GetToDoItemByIdQuery request, CancellationToken cancellationToken)
    {
        var toDoItem = await _genericRepository.GetEntityAsync<ToDoItem>(request.Id, false)
            ?? throw new NotFoundException(nameof(ToDoItem), request.Id.ToString());

        var toDoItemDto = new ToDoItemDto(
            toDoItem.Id, 
            toDoItem.Title, 
            toDoItem.Description,
            toDoItem.ExpiryDateTimeUtc,
            toDoItem.CompletionPercentage
            );

        return toDoItemDto;
    }
}
