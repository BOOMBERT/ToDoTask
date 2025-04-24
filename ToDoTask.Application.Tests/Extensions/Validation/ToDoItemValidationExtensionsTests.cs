using FluentValidation;
using FluentValidation.TestHelper;
using Xunit;

namespace ToDoTask.Application.Extensions.Validation.Tests;

public class ToDoItemValidationExtensionsTests
{
    #region Test_IsValidToDoItemTitle

    private InlineValidator<string> GetIsValidToDoItemTitleValidator()
    {
        var validator = new InlineValidator<string>();
        validator.RuleFor(x => x).IsValidToDoItemTitle();
        return validator;
    }

    [Fact]
    public void IsValidToDoItemTitle_WhenValidToDoItemTitle_ShouldNotHaveValidationErrors()
    {
        // Arrange

        var validToDoItemTitle = "Valid ToDo Item Title";

        // Act

        var result = GetIsValidToDoItemTitleValidator().TestValidate(validToDoItemTitle);

        // Assert

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData("")]
    [InlineData("  ")]
    [InlineData(" a")]
    [InlineData("ab")]
    public void IsValidToDoItemTitle_WhenTooShortToDoItemTitle_ShouldHaveValidationError(string tooShortToDoItemTitle)
    {        
        // Act
        
        var result = GetIsValidToDoItemTitleValidator().TestValidate(tooShortToDoItemTitle);
        
        // Assert

        result.ShouldHaveValidationErrorFor(x => x)
            .WithErrorMessage("Title must be at least 3 characters long.");
    }

    [Fact]
    public void IsValidToDoItemTitle_WhenTooLongToDoItemTitle_ShouldHaveValidationError()
    {
        // Arrange

        var maxLengthOfToDoItemTitle = 128;
        var tooLongToDoItemTitle = new string('A', maxLengthOfToDoItemTitle + 1);

        // Act

        var result = GetIsValidToDoItemTitleValidator().TestValidate(tooLongToDoItemTitle);

        // Assert

        result.ShouldHaveValidationErrorFor(x => x)
            .WithErrorMessage("Title cannot exceed 128 characters.");
    }

    #endregion

    #region Test_IsValidToDoItemDescription

    private InlineValidator<string> GetIsValidToDoItemDescriptionValidator()
    {
        var validator = new InlineValidator<string>();
        validator.RuleFor(x => x).IsValidToDoItemDescription();
        return validator;
    }

    [Theory]
    [InlineData("")]
    [InlineData("Valid ToDo Item Description")]
    public void IsValidToDoItemDescription_WhenValidToDoItemDescription_ShouldNotHaveValidationErrors(string validToDoItemDescription)
    {
        // Act

        var result = GetIsValidToDoItemDescriptionValidator().TestValidate(validToDoItemDescription);

        // Assert

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void IsValidToDoItemDescription_WhenTooLongToDoItemDescription_ShouldHaveValidationError()
    {
        // Arrange

        var maxLengthOfToDoItemDescription = 512;
        var tooLongToDoItemDescription = new string('A', maxLengthOfToDoItemDescription + 1);

        // Act

        var result = GetIsValidToDoItemDescriptionValidator().TestValidate(tooLongToDoItemDescription);

        // Assert

        result.ShouldHaveValidationErrorFor(x => x)
            .WithErrorMessage("Description cannot exceed 512 characters.");
    }

    #endregion
}