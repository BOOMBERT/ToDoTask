using Moq;
using ToDoTask.Application.ToDoItems.Commands.DeleteToDoItem;
using ToDoTask.Domain.Entities;
using ToDoTask.Domain.Exceptions;
using ToDoTask.Domain.Repositories;
using Xunit;

namespace ToDoTask.Application.Tests.ToDoItems.Commands.DeleteToDoItem;

public class DeleteToDoItemCommandHandlerTests
{
    private readonly Mock<IGenericRepository> _genericRepositoryMock = new();

    private readonly DeleteToDoItemCommandHandler _handler;

    public DeleteToDoItemCommandHandlerTests()
    {
        _handler = new DeleteToDoItemCommandHandler(_genericRepositoryMock.Object);
    }

    [Fact]
    public async Task Handle_WhenValidCommand_ShouldDeleteToDoItem()
    {
        // Arrange

        var command = new DeleteToDoItemCommand(Guid.NewGuid());

        _genericRepositoryMock
            .Setup(repo => repo.EntityExistsAsync<ToDoItem>(command.Id))
            .ReturnsAsync(true);

        // Act

        await _handler.Handle(command, CancellationToken.None);

        // Assert

        _genericRepositoryMock.Verify(repo => repo.EntityExistsAsync<ToDoItem>(command.Id), Times.Once);
        _genericRepositoryMock.Verify(repo => repo.DeleteAsync<ToDoItem>(command.Id), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenNonExistingToDoItem_ShouldThrowNotFoundException()
    {
        // Arrange

        var command = new DeleteToDoItemCommand(Guid.NewGuid());

        _genericRepositoryMock
            .Setup(repo => repo.EntityExistsAsync<ToDoItem>(command.Id))
            .ReturnsAsync(false);

        // Act

        Func<Task> action = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert

        var exception = await Assert.ThrowsAsync<NotFoundException>(action);
        Assert.Equal($"ToDoItem with identifier '{command.Id}' was not found", exception.Message);
    }
}