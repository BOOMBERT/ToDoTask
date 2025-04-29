using Moq;
using ToDoTask.Application.Interfaces;
using ToDoTask.Application.ToDoItems.Queries.GetAllToDoItems;
using ToDoTask.Domain.Constants;
using ToDoTask.Domain.Entities;
using ToDoTask.Domain.Repositories;
using Xunit;

namespace ToDoTask.Application.Tests.ToDoItems.Queries.GetAllToDoItems;

public class GetAllToDoItemsQueryHandlerTests
{
    private readonly Mock<IToDoItemsRepository> _toDoItemsRepositoryMock = new();
    private readonly Mock<IClock> _clockMock = new();

    private readonly GetAllToDoItemsQueryHandler _handler;

    public GetAllToDoItemsQueryHandlerTests()
    {
        _handler = new GetAllToDoItemsQueryHandler(_toDoItemsRepositoryMock.Object, _clockMock.Object);
    }

    [Theory]
    [InlineData(null, null)]
    [InlineData("Europe/Warsaw", null)]
    [InlineData(null, DateTimeRange.Today)]
    [InlineData("Europe/Warsaw", DateTimeRange.Today)]
    public async Task Handle_WhenDifferentTimeZoneIdAndDateTimeRangeFilterInputs_ShouldReturnCorrectToDoItemsAndPaginationInfo(string? timeZoneId, DateTimeRange? dateTimeRange)
    {
        // Arrange

        var query = new GetAllToDoItemsQuery
        {
            DateTimeRangeFilter = dateTimeRange,
            TimeZoneId = timeZoneId,
            PageNumber = 1,
            PageSize = 10
        };

        var toDoItem = new ToDoItem
        {
            Id = Guid.NewGuid(),
            Title = "Test Title",
            Description = "Test Description",
            ExpiryDateTimeUtc = DateTime.UtcNow.AddHours(10),
            CompletionPercentage = 50m
        };

        _toDoItemsRepositoryMock
            .Setup(r => r.GetAllMatchingAsync(
                query.SearchPhrase,
                query.PageNumber,
                query.PageSize,
                query.SortBy,
                query.SortDirection,
                It.IsAny<DateTime?>(),
                It.IsAny<DateTime?>()
                ))
            .ReturnsAsync((new List<ToDoItem> { toDoItem }, 1));

        // Act

        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert

        var (resultData, resultPaginationInfo) = (result.Data, result.Pagination);
        var expectedToDoItem = resultData.SingleOrDefault();

        Assert.NotNull(expectedToDoItem);
        Assert.Equal(toDoItem.Id, expectedToDoItem.Id);
        Assert.Equal(toDoItem.Title, expectedToDoItem.Title);
        Assert.Equal(toDoItem.Description, expectedToDoItem.Description);
        Assert.Equal(toDoItem.ExpiryDateTimeUtc, expectedToDoItem.ExpiryDateTimeUtc);
        Assert.Equal(toDoItem.CompletionPercentage, expectedToDoItem.CompletionPercentage);

        Assert.Equal(1, resultPaginationInfo.TotalItemCount);
        Assert.Equal(1, resultPaginationInfo.TotalPageCount);
        Assert.Equal(query.PageSize, resultPaginationInfo.PageSize);
        Assert.Equal(query.PageNumber, resultPaginationInfo.CurrentPage);

        if (timeZoneId != null && dateTimeRange != null)
        {
            _clockMock.Verify(c => c.UtcNow, Times.Once);
        }
        else
        {
            _clockMock.Verify(c => c.UtcNow, Times.Never);
        }

        _toDoItemsRepositoryMock.Verify(r => r.GetAllMatchingAsync(
            query.SearchPhrase,
            query.PageNumber,
            query.PageSize,
            query.SortBy,
            query.SortDirection,
            It.IsAny<DateTime?>(),
            It.IsAny<DateTime?>()
            ), Times.Once);
    }
}