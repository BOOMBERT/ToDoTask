using MediatR;
using ToDoTask.Domain.Entities;
using ToDoTask.Domain.Exceptions;
using ToDoTask.Domain.Repositories;

namespace ToDoTask.Application.ToDoItems.Commands.SetToDoItemCompletionPercentage;

public class SetToDoItemCompletionPercentageCommandHandler : IRequestHandler<SetToDoItemCompletionPercentageCommand>
{
    private readonly IGenericRepository _genericRepository;
    private readonly IToDoItemsRepository _toDoItemsRepository;

    public SetToDoItemCompletionPercentageCommandHandler(IGenericRepository genericRepository, IToDoItemsRepository toDoItemsRepository)
    {
        _genericRepository = genericRepository;
        _toDoItemsRepository = toDoItemsRepository;
    }

    public async Task Handle(SetToDoItemCompletionPercentageCommand request, CancellationToken cancellationToken)
    {
        if (!await _genericRepository.EntityExistsAsync<ToDoItem>(request.Id))
            throw new NotFoundException(nameof(ToDoItem), request.Id.ToString());

        await _toDoItemsRepository.UpdateCompletionPercentageAsync(request.Id, request.CompletionPercentage);
    }
}
