using System;
using System.Threading.Tasks;

using Application.Features.Auth.Command.Register;

using FluentValidation;

using Xunit;

namespace Application.Test.Auth.CommandTest.RegisterOperator;

public class RegisterOperationValidatorTest
{
    private readonly RegisterOperatorCommandValidator _validator;

    public RegisterOperationValidatorTest()
    {
        _validator = new RegisterOperatorCommandValidator();
    }

    private RegisterOperatorCommand CreateValidCommand(
        string email = "operator@example.com",
        string password = "Valid1@Password!",
        string firstName = "Alice",
        string lastName = "Smith",
        DateTime? hireDate = null,
        bool isActive = true
    )
    {
        return new RegisterOperatorCommand(
            email,
            password,
            firstName,
            lastName,
            hireDate ?? DateTime.UtcNow.Date,
            isActive
        );
    }

    [Fact]
    public async Task Validator_Should_Pass_With_Valid_Command()
    {
        var command = CreateValidCommand();
        var result = await _validator.ValidateAsync(command);
        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public async Task Validator_Should_Fail_When_Email_Is_Empty(string? invalidEmail)
    {
        var command = CreateValidCommand(email: invalidEmail);
        var result = await _validator.ValidateAsync(command);
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Email");
    }

    [Theory]
    [InlineData("not-an-email")]
    [InlineData("missingdomain.com")]
    public async Task Validator_Should_Fail_When_Email_Is_Invalid(string invalidEmail)
    {
        var command = CreateValidCommand(email: invalidEmail);
        var result = await _validator.ValidateAsync(command);
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Email");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public async Task Validator_Should_Fail_When_Password_Is_Empty(string invalidPassword)
    {
        var command = CreateValidCommand(password: invalidPassword);
        var result = await _validator.ValidateAsync(command);
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Password");
    }

    [Theory]
    [InlineData("short")]
    [InlineData("abc")]
    [InlineData("12345")]
    public async Task Validator_Should_Fail_When_Password_Is_Too_Short(string invalidPassword)
    {
        var command = CreateValidCommand(password: invalidPassword);
        var result = await _validator.ValidateAsync(command);
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Password" && e.ErrorMessage.Contains("at least 6 characters"));
    }

    [Theory]
    [InlineData("alllowercase1@")]
    [InlineData("ALLUPPERCASE1@")]
    [InlineData("NoNumber@")]
    [InlineData("NoSpecial1")]
    public async Task Validator_Should_Fail_When_Password_Missing_Requirements(string invalidPassword)
    {
        var command = CreateValidCommand(password: invalidPassword);
        var result = await _validator.ValidateAsync(command);
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Password");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public async Task Validator_Should_Fail_When_FirstName_Is_Empty(string? invalidFirstName)
    {
        var command = CreateValidCommand(firstName: invalidFirstName);
        var result = await _validator.ValidateAsync(command);
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "FirstName");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public async Task Validator_Should_Fail_When_LastName_Is_Empty(string? invalidLastName)
    {
        var command = CreateValidCommand(lastName: invalidLastName);
        var result = await _validator.ValidateAsync(command);
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "LastName");
    }

    [Theory]
    [InlineData("1899-12-31")]
    [InlineData("3000-01-01")]
    public async Task Validator_Should_Fail_When_HireDate_Is_Invalid(string dateString)
    {
        var invalidDate = DateTime.Parse(dateString);
        var command = CreateValidCommand(hireDate: invalidDate);
        var result = await _validator.ValidateAsync(command);
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "HireDate");
    }
}