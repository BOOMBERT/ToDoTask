using MediatR;
using ToDoTask.Domain.Entities;
using ToDoTask.Domain.Exceptions;
using ToDoTask.Domain.Repositories;

namespace ToDoTask.Application.ToDoItems.Commands.DeleteToDoItem;

public class DeleteToDoItemCommandHandler : IRequestHandler<DeleteToDoItemCommand>
{
    private readonly IGenericRepository _genericRepository;

    public DeleteToDoItemCommandHandler(IGenericRepository genericRepository)
    {
        _genericRepository = genericRepository;
    }

    public async Task Handle(DeleteToDoItemCommand request, CancellationToken cancellationToken)
    {
        if (!await _genericRepository.EntityExistsAsync<ToDoItem>(request.Id))
            throw new NotFoundException(nameof(ToDoItem), request.Id.ToString());

        await _genericRepository.DeleteAsync<ToDoItem>(request.Id);
    }
}
