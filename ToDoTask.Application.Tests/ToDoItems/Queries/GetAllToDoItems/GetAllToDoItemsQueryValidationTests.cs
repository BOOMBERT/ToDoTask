using FluentValidation.TestHelper;
using ToDoTask.Application.ToDoItems.Queries.GetAllToDoItems;
using ToDoTask.Domain.Constants;
using Xunit;

namespace ToDoTask.Application.Tests.ToDoItems.Queries.GetAllToDoItems;

public class GetAllToDoItemsQueryValidationTests
{
    private (GetAllToDoItemsQuery, GetAllToDoItemsQueryValidation) GetAllToDoItemsQueryAndValidation(DateTimeRange? dateTimeRange, string? timeZoneId) 
        => (new GetAllToDoItemsQuery { DateTimeRangeFilter = dateTimeRange, TimeZoneId = timeZoneId}, new GetAllToDoItemsQueryValidation());

    [Theory]
    [InlineData(null, null)]
    [InlineData(DateTimeRange.Tomorrow, "Europe/Warsaw")]
    public void Validation_WhenValidQuery_ShouldNotHaveValidationErrors(DateTimeRange? dateTimeRange, string? timeZoneId)
    {
        // Arrange

        var (query, validator) = GetAllToDoItemsQueryAndValidation(dateTimeRange, timeZoneId);

        // Act

        var result = validator.TestValidate(query);

        // Assert

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData(null, "America/New_York")]
    [InlineData(DateTimeRange.ThisWeek, null)]
    public void Validation_WhenOnlyOneOfDateTimeRangeFilterOrTimeZoneIdIsProvidedAndValid_ShouldHaveValidationErrors(DateTimeRange? dateTimeRange, string? timeZoneId)
    {
        // Arrange

        var (query, validator) = GetAllToDoItemsQueryAndValidation(dateTimeRange, timeZoneId);

        // Act

        var result = validator.TestValidate(query);

        // Assert

        result.ShouldHaveValidationErrorFor(q => q.DateTimeRangeFilter)
            .WithErrorMessage("DateTimeRangeFilter must be specified when TimeZoneId is provided, or must be null if TimeZoneId is null.");

        result.ShouldHaveValidationErrorFor(q => q.TimeZoneId)
            .WithErrorMessage("TimeZoneId must be specified when DateTimeRangeFilter is provided, or must be null if DateTimeRangeFilter is null.");
    }

    [Fact]
    public void Validation_WhenInvalidTimeZoneId_ShouldHaveValidationError()
    {
        // Arrange

        var (query, validator) = GetAllToDoItemsQueryAndValidation(DateTimeRange.Today, "Invalid/TimeZone");

        // Act

        var result = validator.TestValidate(query);

        // Assert

        result.ShouldHaveValidationErrorFor(q => q.TimeZoneId)
            .WithErrorMessage("Invalid time zone id (e.g., 'Europe/Warsaw', 'America/New_York').");
    }
}