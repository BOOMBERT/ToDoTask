namespace ToDoTask.Domain.Repositories;

public interface IToDoItemsRepository
{
    Task UpdateCompletionPercentageAsync(Guid id, decimal completionPercentage);
}
