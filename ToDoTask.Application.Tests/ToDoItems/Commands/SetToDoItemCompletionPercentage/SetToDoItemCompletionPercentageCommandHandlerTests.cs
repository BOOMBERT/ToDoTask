using Moq;
using ToDoTask.Application.ToDoItems.Commands.SetToDoItemCompletionPercentage;
using ToDoTask.Domain.Entities;
using ToDoTask.Domain.Exceptions;
using ToDoTask.Domain.Repositories;
using Xunit;

namespace ToDoTask.Application.Tests.ToDoItems.Commands.SetToDoItemCompletionPercentage;

public class SetToDoItemCompletionPercentageCommandHandlerTests
{
    private readonly Mock<IGenericRepository> _genericRepositoryMock = new();
    private readonly Mock<IToDoItemsRepository> _toDoItemsRepositoryMock = new();

    private readonly SetToDoItemCompletionPercentageCommandHandler _handler;

    public SetToDoItemCompletionPercentageCommandHandlerTests()
    {
        _handler = new SetToDoItemCompletionPercentageCommandHandler(_genericRepositoryMock.Object, _toDoItemsRepositoryMock.Object);
    }

    [Fact]
    public async Task Handle_WhenValidCommand_ShouldSetToDoItemCompletionPercentage()
    {
        // Arrange

        var command = new SetToDoItemCompletionPercentageCommand
        {
            Id = Guid.NewGuid(),
            CompletionPercentage = 50
        };

        _genericRepositoryMock
            .Setup(r => r.EntityExistsAsync<ToDoItem>(command.Id))
            .ReturnsAsync(true);

        // Act

        await _handler.Handle(command, CancellationToken.None);

        // Assert

        _genericRepositoryMock.Verify(r => r.EntityExistsAsync<ToDoItem>(command.Id), Times.Once);
        _toDoItemsRepositoryMock.Verify(r => r.UpdateCompletionPercentageAsync(command.Id, command.CompletionPercentage), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenNonExistingToDoItem_ShouldThrowNotFoundException()
    {
        // Arrange

        var command = new SetToDoItemCompletionPercentageCommand
        {
            Id = Guid.NewGuid(),
            CompletionPercentage = 50
        };
     
        _genericRepositoryMock
            .Setup(r => r.EntityExistsAsync<ToDoItem>(command.Id))
            .ReturnsAsync(false);

        // Act

        Func<Task> action = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert

        var exception = await Assert.ThrowsAsync<NotFoundException>(action);
        Assert.Equal($"ToDoItem with identifier '{command.Id}' was not found", exception.Message);
    }
}