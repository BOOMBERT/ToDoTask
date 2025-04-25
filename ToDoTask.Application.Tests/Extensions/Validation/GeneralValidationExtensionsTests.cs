using FluentValidation;
using FluentValidation.TestHelper;
using Xunit;

namespace ToDoTask.Application.Extensions.Validation.Tests;

public class GeneralValidationExtensionsTests
{
    #region Test_IsFutureUtc

    private InlineValidator<DateTime> GetIsFutureUtcValidator()
    {
        var validator = new InlineValidator<DateTime>();
        validator.RuleFor(x => x).IsFutureUtc();
        return validator;
    }

    [Theory]
    [InlineData(1)] // 1 minute
    [InlineData(60)] // 1 hour
    [InlineData(1440)] // 1 day
    public void IsFutureUtc_WhenFutureUtcDateTime_ShouldNotHaveValidationErrors(int minutesOffset)
    {
        // Arrange

        var futureUtcDateTime = DateTime.UtcNow.AddMinutes(minutesOffset);

        // Act

        var result = GetIsFutureUtcValidator().TestValidate(futureUtcDateTime);

        // Assert

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData(0)] // now
    [InlineData(-1)] // 1 minute ago
    [InlineData(-60)] // 1 hour ago
    [InlineData(-1440)] // 1 day ago
    public void IsFutureUtc_WhenNonFutureUtcDateTime_ShouldHaveValidationError(int minutesOffset)
    {
        // Arrange

        var nonFutureUtcDateTime = DateTime.UtcNow.AddMinutes(minutesOffset);

        // Act

        var result = GetIsFutureUtcValidator().TestValidate(nonFutureUtcDateTime);

        // Assert

        result.ShouldHaveValidationErrorFor(x => x)
            .WithErrorMessage("Date time must be in the future.");
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