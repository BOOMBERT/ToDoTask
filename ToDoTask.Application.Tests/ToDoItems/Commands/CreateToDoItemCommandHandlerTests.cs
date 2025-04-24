using Moq;
using ToDoTask.Domain.Entities;
using ToDoTask.Domain.Repositories;
using Xunit;

namespace ToDoTask.Application.ToDoItems.Commands.Tests;

public class CreateToDoItemCommandHandlerTests
{
    private readonly Mock<IGenericRepository> _genericRepositoryMock = new();

    private readonly CreateToDoItemCommandHandler _handler;

    public CreateToDoItemCommandHandlerTests()
    {
        _handler = new CreateToDoItemCommandHandler(_genericRepositoryMock.Object);    
    }

    [Fact]
    public async Task Handle_WhenValidCommand_ShouldReturnNonEmptyGuid()
    {
        // Arrange

        var command = new CreateToDoItemCommand
        {
            Title = "Test Title",
            Description = "Test Description",
            ExpiryDateTime = DateTime.Now.AddDays(1),
            CompletionPercentage = 0
        };

        _genericRepositoryMock
            .Setup(r => r.AddAsync(It.IsAny<ToDoItem>()))
            .Returns(Task.CompletedTask);

        _genericRepositoryMock
            .Setup(r => r.SaveChangesAsync())
            .Returns(Task.FromResult(true));

        // Act

        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert

        Assert.NotEqual(Guid.Empty, result);
        _genericRepositoryMock.Verify(r => r.AddAsync(It.IsAny<ToDoItem>()), Times.Once);
        _genericRepositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
    }
}