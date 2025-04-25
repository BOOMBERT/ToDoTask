using Moq;
using ToDoTask.Application.ToDoItems.Queries.GetToDoItemById;
using ToDoTask.Domain.Entities;
using ToDoTask.Domain.Exceptions;
using ToDoTask.Domain.Repositories;
using Xunit;

namespace ToDoTask.Application.Tests.ToDoItems.Queries.GetToDoItemById;

public class GetToDoItemByIdQueryHandlerTests
{
    private readonly Mock<IGenericRepository> _genericRepositoryMock = new();

    private readonly GetToDoItemByIdQueryHandler _handler;

    public GetToDoItemByIdQueryHandlerTests()
    {
        _handler = new GetToDoItemByIdQueryHandler(_genericRepositoryMock.Object);
    }

    [Fact]
    public async Task Handle_WhenValidQuery_ShouldReturnToDoItemDto()
    {
        // Arrange

        var query = new GetToDoItemByIdQuery(Guid.NewGuid());

        var toDoItem = new ToDoItem
        {
            Id = query.Id,
            Title = "Test Title",
            Description = "Test Description",
            ExpiryDateTimeUtc = DateTime.UtcNow.AddDays(1),
            CompletionPercentage = 50
        };

        _genericRepositoryMock
            .Setup(r => r.GetEntityAsync<ToDoItem>(query.Id, false))
            .ReturnsAsync(toDoItem);

        // Act

        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert

        Assert.NotNull(result);
        Assert.Equal(toDoItem.Id, result.Id);
        Assert.Equal(toDoItem.Title, result.Title);
        Assert.Equal(toDoItem.Description, result.Description);
        Assert.Equal(toDoItem.ExpiryDateTimeUtc, result.ExpiryDateTimeUtc);
        Assert.Equal(toDoItem.CompletionPercentage, result.CompletionPercentage);

        _genericRepositoryMock.Verify(r => r.GetEntityAsync<ToDoItem>(query.Id, false), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenNonExistingToDoItem_ShouldThrowNotFoundException()
    {
        // Arrange

        var query = new GetToDoItemByIdQuery(Guid.Empty);

        _genericRepositoryMock
            .Setup(r => r.GetEntityAsync<ToDoItem>(query.Id, false))
            .ReturnsAsync((ToDoItem)null!);

        // Act

        Func<Task> action = async () => await _handler.Handle(query, CancellationToken.None);

        // Assert

        var exception = await Assert.ThrowsAsync<NotFoundException>(action);
        Assert.Equal($"ToDoItem with identifier '{Guid.Empty}' was not found", exception.Message);
    }
}