using FluentValidation;
using FluentValidation.TestHelper;
using ToDoTask.Application.Extensions.Validation;
using ToDoTask.Application.Interfaces;
using ToDoTask.Application.Tests.Helpers;
using ToDoTask.Domain.Constants;
using Xunit;

namespace ToDoTask.Application.Tests.Extensions.Validation;

public class PaginationAndSortingValidationExtensionsTests
{
    #region Test_IsValidPageNumber

    private InlineValidator<IPaginationAndSortingQuery> GetIsValidPageNumberValidator()
    {
        var validator = new InlineValidator<IPaginationAndSortingQuery>();
        validator.RuleFor(x => x.PageNumber).IsValidPageNumber();

        return validator;
    }

    [Theory]
    [InlineData(1)]
    [InlineData(12)]
    public void IsValidPageNumber_WhenValidPageNumber_ShouldNotHaveValidationErrors(int pageNumber)
    {
        // Arrange

        var paginationQuery = new PaginationAndSortingQuery { PageNumber = pageNumber };

        // Act

        var result = GetIsValidPageNumberValidator().TestValidate(paginationQuery);

        // Assert

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void IsValidPageNumber_WhenInvalidPageNumber_ShouldHaveValidationError(int pageNumber)
    {
        // Arrange

        var paginationQuery = new PaginationAndSortingQuery { PageNumber = pageNumber };

        // Act

        var result = GetIsValidPageNumberValidator().TestValidate(paginationQuery);

        // Assert

        result.ShouldHaveValidationErrorFor(x => x.PageNumber)
            .WithErrorMessage("Page number must be greater than or equal to 1.");
    }

    #endregion

    #region Test_IsValidPageSize

    private readonly int[] allowedPageSizes = { 5, 10, 20, 50 };

    private InlineValidator<IPaginationAndSortingQuery> GetIsValidPageSizeValidator()
    {
        var validator = new InlineValidator<IPaginationAndSortingQuery>();
        validator.RuleFor(x => x.PageSize).IsValidPageSize(allowedPageSizes);

        return validator;
    }

    [Theory]
    [InlineData(5)]
    [InlineData(50)]
    public void IsValidPageSize_WhenValidPageSize_ShouldNotHaveValidationErrors(int pageSize)
    {
        // Arrange

        var paginationQuery = new PaginationAndSortingQuery { PageSize = pageSize };

        // Act

        var result = GetIsValidPageSizeValidator().TestValidate(paginationQuery);

        // Assert

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(12)]
    public void IsValidPageSize_WhenInvalidPageSize_ShouldHaveValidationError(int pageSize)
    {
        // Arrange

        var paginationQuery = new PaginationAndSortingQuery { PageSize = pageSize };

        // Act

        var result = GetIsValidPageSizeValidator().TestValidate(paginationQuery);

        // Assert

        result.ShouldHaveValidationErrorFor(x => x.PageSize)
            .WithErrorMessage($"Page size must be in [{string.Join(",", allowedPageSizes)}].");
    }

    #endregion

    #region Test_IsValidSortBy_And_IsValidSortDirection

    private readonly string[] allowedSortByColumnNames = { "test 1", "test 2" };

    private InlineValidator<IPaginationAndSortingQuery> GetIsValidSortByValidator()
    {
        var validator = new InlineValidator<IPaginationAndSortingQuery>();
        validator.RuleFor(x => x.SortBy).IsValidSortBy(allowedSortByColumnNames);

        return validator;
    }

    private InlineValidator<IPaginationAndSortingQuery> GetIsValidSortDirectionValidator()
    {
        var validator = new InlineValidator<IPaginationAndSortingQuery>();
        validator.RuleFor(x => x.SortDirection).IsValidSortDirection();

        return validator;
    }

    [Theory]
    [InlineData("test 1", SortDirection.Ascending)]
    [InlineData("Test 1", SortDirection.Descending)]
    [InlineData("test 2", SortDirection.Ascending)]
    [InlineData(null, null)]
    public void IsValidSortByAndIsValidSortDirection_WhenValidSortByAndSortDirection_ShouldNotHaveValidationErrors(string? sortBy, SortDirection? sortDirection)
    {
        // Arrange

        var paginationQuery = new PaginationAndSortingQuery { SortBy = sortBy, SortDirection = sortDirection };

        // Act

        var resultSortBy = GetIsValidSortByValidator().TestValidate(paginationQuery);
        var resultSortDirection = GetIsValidSortDirectionValidator().TestValidate(paginationQuery);

        // Assert

        resultSortBy.ShouldNotHaveAnyValidationErrors();
        resultSortDirection.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData("test 12", SortDirection.Ascending)]
    [InlineData("test 3", SortDirection.Descending)]
    public void IsValidSortByAndIsValidSortDirection_WhenInvalidSortByAndValidSortDirection_ShouldHaveValidationError(string? sortBy, SortDirection? sortDirection)
    {
        // Arrange

        var paginationQuery = new PaginationAndSortingQuery { SortBy = sortBy, SortDirection = sortDirection };

        // Act

        var resultSortBy = GetIsValidSortByValidator().TestValidate(paginationQuery);
        var resultSortDirection = GetIsValidSortDirectionValidator().TestValidate(paginationQuery);

        // Assert

        resultSortBy.ShouldHaveValidationErrorFor(x => x.SortBy)
            .WithErrorMessage("SortBy must be null if SortDirection is null, or must be one of [test 1, test 2] when SortDirection is specified.");

        resultSortDirection.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData("test 1", null)]
    [InlineData(null, SortDirection.Ascending)]
    public void IsValidSortByAndIsValidSortDirection_WhenOnlyOneOfSortByOrSortDirectionIsProvidedAndValid_ShouldHaveValidationErrors(string? sortBy, SortDirection? sortDirection)
    {
        // Arrange

        var paginationQuery = new PaginationAndSortingQuery { SortBy = sortBy, SortDirection = sortDirection };

        // Act

        var resultSortBy = GetIsValidSortByValidator().TestValidate(paginationQuery);
        var resultSortDirection = GetIsValidSortDirectionValidator().TestValidate(paginationQuery);

        // Assert

        resultSortBy.ShouldHaveValidationErrorFor(x => x.SortBy)
            .WithErrorMessage("SortBy must be null if SortDirection is null, or must be one of [test 1, test 2] when SortDirection is specified.");

        resultSortDirection.ShouldHaveValidationErrorFor(x => x.SortDirection)
            .WithErrorMessage("SortDirection must be provided if SortBy is provided, or must be null if SortBy is null.");
    }

    #endregion
}