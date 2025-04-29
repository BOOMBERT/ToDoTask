using Moq;
using ToDoTask.Application.Interfaces;
using ToDoTask.Application.Utils;
using ToDoTask.Domain.Constants;
using Xunit;

namespace ToDoTask.Application.Tests.Utils;

public class DateTimeUtilTests
{
    private readonly Mock<IClock> _clockMock = new();

    private const string PacificHonoluluTimeZoneId = "Pacific/Honolulu"; // Constant difference of -10 hours from UTC

    [Theory]
    [InlineData(DateTimeRange.Today, "2025-03-31T10:00:00Z", "2025-04-01T10:00:00Z")]
    [InlineData(DateTimeRange.Tomorrow, "2025-04-01T10:00:00Z", "2025-04-02T10:00:00Z")]
    [InlineData(DateTimeRange.ThisWeek, "2025-03-31T10:00:00Z", "2025-04-07T10:00:00Z")]
    public void GetUtcDateRange_ForGivenDateTimeRange_ShouldReturnCorrectUtcStartAndEnd(DateTimeRange dateTimeRange, string expectedUtcStartString, string expectedUtcEndString)
    {
        // Arrange

        var expectedUtcStart = DateTime.Parse(expectedUtcStartString).ToUniversalTime();
        var expectedUtcEnd = DateTime.Parse(expectedUtcEndString).ToUniversalTime();

        var fixedUtcNow = new DateTime(2025, 4, 01, 05, 30, 15, DateTimeKind.Utc);

        _clockMock
            .Setup(clock => clock.UtcNow)
            .Returns(fixedUtcNow);

        var timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(PacificHonoluluTimeZoneId);

        // Act

        var (utcStart, utcEnd) = DateTimeUtil.GetUtcDateRange(dateTimeRange, timeZoneInfo, _clockMock.Object);

        // Assert

        Assert.Equal(expectedUtcStart, utcStart);
        Assert.Equal(expectedUtcEnd, utcEnd);
    }
}