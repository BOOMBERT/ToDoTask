using MediatR;
using ToDoTask.Domain.Entities;
using ToDoTask.Domain.Repositories;

namespace ToDoTask.Application.ToDoItems.Commands.CreateToDoItem;

public class CreateToDoItemCommandHandler : IRequestHandler<CreateToDoItemCommand, Guid>
{
    private readonly IGenericRepository _genericRepository;

    public CreateToDoItemCommandHandler(IGenericRepository genericRepository)
    {
        _genericRepository = genericRepository;
    }

    public async Task<Guid> Handle(CreateToDoItemCommand request, CancellationToken cancellationToken)
    {
        var toDoItem = new ToDoItem 
        {
            Id = Guid.NewGuid(),
            Title = request.Title, 
            Description = request.Description, 
            ExpiryDateTime = request.ExpiryDateTime.ToUniversalTime(), 
            CompletionPercentage = request.CompletionPercentage 
        };

        await _genericRepository.AddAsync(toDoItem);
        await _genericRepository.SaveChangesAsync();

        return toDoItem.Id;
    }
}
