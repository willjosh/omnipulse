using Application.Features.Auth.Command;

namespace Application.Test.Auth.CommandTest.Login;

public class LoginCommandValidatorTest
{
    private readonly LoginCommandValidator _validator;

    public LoginCommandValidatorTest()
    {
        _validator = new LoginCommandValidator();
    }

    private LoginCommand CreateValidCommand(
        string email = "test@example.com",
        string password = "Valid1@Password"
    )
    {
        return new LoginCommand(email, password);
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
    [InlineData(null)]
    public async Task Validator_Should_Fail_When_Password_Is_Empty(string? invalidPassword)
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
}