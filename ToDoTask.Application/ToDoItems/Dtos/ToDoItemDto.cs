namespace ToDoTask.Application.ToDoItems.Dtos;

public record ToDoItemDto(
    Guid Id, 
    string Title, 
    string Description, 
    DateTime ExpiryDateTimeUtc, 
    decimal CompletionPercentage
    );
