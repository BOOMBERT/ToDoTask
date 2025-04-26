using Moq;
using ToDoTask.Application.ToDoItems.Commands.UpdateToDoItem;
using ToDoTask.Domain.Entities;
using ToDoTask.Domain.Exceptions;
using ToDoTask.Domain.Repositories;
using Xunit;

namespace ToDoTask.Application.Tests.ToDoItems.Commands.UpdateToDoItem;

public class UpdateToDoItemCommandHandlerTests
{
    private readonly Mock<IGenericRepository> _genericRepositoryMock = new();

    private readonly UpdateToDoItemCommandHandler _handler;

    public UpdateToDoItemCommandHandlerTests()
    {
        _handler = new UpdateToDoItemCommandHandler(_genericRepositoryMock.Object);
    }

    [Fact]
    public async Task Handle_WhenValidCommand_ShouldUpdateToDoItem()
    {
        // Arrange

        var toDoItem = new ToDoItem
        {
            Id = Guid.NewGuid(),
            Title = "Test Title",
            Description = "Test Description",
            ExpiryDateTimeUtc = DateTime.UtcNow.AddMinutes(30),
            CompletionPercentage = 0
        };

        var command = new UpdateToDoItemCommand(toDoItem.Id)
        {
            Title = "Updated Title",
            Description = "Updated Description",
            ExpiryDateTimeUtc = DateTime.UtcNow.AddDays(1),
            CompletionPercentage = 50
        };

        _genericRepositoryMock
            .Setup(r => r.GetEntityAsync<ToDoItem>(command.Id, true))
            .ReturnsAsync(toDoItem);

        _genericRepositoryMock
            .Setup(r => r.SaveChangesAsync())
            .Returns(Task.FromResult(true));

        // Act

        await _handler.Handle(command, CancellationToken.None);

        // Assert

        Assert.Equal(command.Title, toDoItem.Title);
        Assert.Equal(command.Description, toDoItem.Description);
        Assert.Equal(command.ExpiryDateTimeUtc, toDoItem.ExpiryDateTimeUtc);
        Assert.Equal(command.CompletionPercentage, toDoItem.CompletionPercentage);

        _genericRepositoryMock.Verify(r => r.GetEntityAsync<ToDoItem>(command.Id, true), Times.Once);
        _genericRepositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenNonExistingToDoItem_ShouldThrowNotFoundException()
    {
        // Arrange

        var command = new UpdateToDoItemCommand(Guid.NewGuid())
        {
            Title = "Updated Title",
            Description = "Updated Description",
            ExpiryDateTimeUtc = DateTime.UtcNow.AddDays(1),
            CompletionPercentage = 50
        };

        _genericRepositoryMock
            .Setup(r => r.GetEntityAsync<ToDoItem>(command.Id, true))
            .ReturnsAsync((ToDoItem)null!);

        // Act

        Func<Task> action = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert

        var exception = await Assert.ThrowsAsync<NotFoundException>(action);
        Assert.Equal($"ToDoItem with identifier '{command.Id}' was not found", exception.Message);
    }
}