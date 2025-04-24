using FluentValidation;
using FluentValidation.TestHelper;
using Xunit;

namespace ToDoTask.Application.Extensions.Validation.Tests;

public class GeneralValidationExtensionsTests
{
    #region Test_IsNotPastOrPresent

    private InlineValidator<DateTimeOffset> GetIsNotPastOrPresentValidator()
    {
        var validator = new InlineValidator<DateTimeOffset>();
        validator.RuleFor(x => x).IsNotPastOrPresent();
        return validator;
    }

    [Theory]
    [InlineData(1)] // 1 minute
    [InlineData(60)] // 1 hour
    [InlineData(1440)] // 1 day
    public void IsNotPastOrPresent_WhenFutureDateTime_ShouldNotHaveValidationErrors(int minutesOffset)
    {
        // Arrange

        var futureDateTime = DateTimeOffset.Now.AddMinutes(minutesOffset);

        // Act

        var result = GetIsNotPastOrPresentValidator().TestValidate(futureDateTime);

        // Assert

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData(0)] // now
    [InlineData(-1)] // 1 minute ago
    [InlineData(-60)] // 1 hour ago
    [InlineData(-1440)] // 1 day ago
    public void IsNotPastOrPresent_WhenPastOrPresentDateTime_ShouldHaveValidationError(int minutesOffset)
    {
        // Arrange

        var pastOrPresentDateTime = DateTimeOffset.Now.AddMinutes(minutesOffset);

        // Act

        var result = GetIsNotPastOrPresentValidator().TestValidate(pastOrPresentDateTime);

        // Assert

        result.ShouldHaveValidationErrorFor(x => x)
            .WithErrorMessage("Date time cannot be in the past or present.");
    }

    #endregion

    #region Test_IsPercentBetween0And100WithMaxTwoDecimals

    private InlineValidator<decimal> GetIsPercentBetween0And100WithMaxTwoDecimalsValidator()
    {
        var validator = new InlineValidator<decimal>();
        validator.RuleFor(x => x).IsPercentBetween0And100WithMaxTwoDecimals();
        return validator;
    }

    [Theory]
    [InlineData(0)]
    [InlineData(100)]
    [InlineData(0.1)]
    [InlineData(1.23)]
    [InlineData(99.90)]
    public void IsPercentBetween0And100WithMaxTwoDecimals_WhenValidPercent_ShouldNotHaveValidationErrors(decimal validPercent)
    {
        // Act

        var result = GetIsPercentBetween0And100WithMaxTwoDecimalsValidator().TestValidate(validPercent);
        
        // Assert
        
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData(-0.01)]
    [InlineData(100.01)]
    [InlineData(1000)]
    public void IsPercentBetween0And100WithMaxTwoDecimals_WhenPercentIsNotBetween0And100_ShouldHaveValidationError(decimal invalidPercent)
    {
        // Act

        var result = GetIsPercentBetween0And100WithMaxTwoDecimalsValidator().TestValidate(invalidPercent);

        // Assert

        result.ShouldHaveValidationErrorFor(x => x)
            .WithErrorMessage("Percent must be between 0 and 100.");
    }

    [Theory]
    [InlineData(0.001)]
    [InlineData(99.101)]
    [InlineData(12.3456789)]
    public void IsPercentBetween0And100WithMaxTwoDecimals_WhenPercentExceedsTwoDecimalPlaces_ShouldHaveValidationError(decimal invalidPercent)
    {
        // Act

        var result = GetIsPercentBetween0And100WithMaxTwoDecimalsValidator().TestValidate(invalidPercent);

        // Assert

        result.ShouldHaveValidationErrorFor(x => x)
            .WithErrorMessage("Percent can have a maximum of two decimal places.");
    }

    #endregion
}