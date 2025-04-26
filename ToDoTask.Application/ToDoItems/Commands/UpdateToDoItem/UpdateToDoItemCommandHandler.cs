using MediatR;
using ToDoTask.Domain.Entities;
using ToDoTask.Domain.Exceptions;
using ToDoTask.Domain.Repositories;

namespace ToDoTask.Application.ToDoItems.Commands.UpdateToDoItem;

public class UpdateToDoItemCommandHandler : IRequestHandler<UpdateToDoItemCommand>
{
    private readonly IGenericRepository _genericRepository;

    public UpdateToDoItemCommandHandler(IGenericRepository genericRepository)
    {
        _genericRepository = genericRepository;
    }

    public async Task Handle(UpdateToDoItemCommand request, CancellationToken cancellationToken)
    {
        var toDoItem = await _genericRepository.GetEntityAsync<ToDoItem>(request.Id)
            ?? throw new NotFoundException(nameof(ToDoItem), request.Id.ToString());

        toDoItem.Title = request.Title;
        toDoItem.Description = request.Description;
        toDoItem.ExpiryDateTimeUtc = request.ExpiryDateTimeUtc;
        toDoItem.CompletionPercentage = request.CompletionPercentage;

        await _genericRepository.SaveChangesAsync();
    }
}
