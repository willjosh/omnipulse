using Application.Features.Users.Command.CreateTechnician;

using FluentValidation;

namespace Application.Test.Users.CommandTest.CreateTechnicianTest;

public class CreateTechnicianCommandValidatorTest
{
    readonly IValidator<CreateTechnicianCommand> _validator;
    public CreateTechnicianCommandValidatorTest()
    {
        _validator = new CreateTechnicianCommandValidator();
    }

    private CreateTechnicianCommand CreateValidCommand(
        string email = "clarenceBowo@gmail.com",
        string password = "clarenceBowo123!",
        string firstName = "Clarence",
        string lastName = "Muljadi",
        DateTime? hireDate = null,
        bool isActive = true
    )
    {
        return new CreateTechnicianCommand(
             email,
             password,
             firstName,
             lastName,
             hireDate ?? DateTime.UtcNow,
             isActive
         );
    }

    [Fact]
    public async Task Validator_Should_Validate_Valid_Command()
    {
        // Given
        var command = CreateValidCommand();

        // When
        var result = await _validator.ValidateAsync(command);

        // Then
        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }

    [Theory]
    [InlineData("invalid-email")]
    [InlineData("email@")]
    public async Task Validator_Should_Fail_On_Invalid_Email(string email)
    {
        // Given
        var command = CreateValidCommand(email: email);

        // When
        var result = await _validator.ValidateAsync(command);

        // Then
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Email");
    }

    [Theory]
    [InlineData("")]                    // Empty password
    [InlineData(" ")]                   // Whitespace only
    [InlineData("short")]               // Too short (assuming min length requirement)
    [InlineData("nouppercase123!")]     // No uppercase letters
    [InlineData("NOLOWERCASE123!")]     // No lowercase letters
    [InlineData("NoNumbers!")]          // No numbers
    [InlineData("NoSpecialChars123")]   // No special characters
    [InlineData("abc")]                 // Way too short
    [InlineData("12345678")]            // Numbers only
    [InlineData("abcdefgh")]            // Letters only
    [InlineData("ABCDEFGH")]            // Uppercase only
    [InlineData("!@#$%^&*")]            // Special characters only
    [InlineData("password")]            // Common weak password
    [InlineData("123456789")]           // Sequential numbers
    [InlineData("qwerty123")]           // Common keyboard pattern
    public async Task Validator_Should_Fail_On_Invalid_Password(string password)
    {
        // Given
        var command = CreateValidCommand(password: password);

        // When
        var result = await _validator.ValidateAsync(command);

        // Then
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Password");
    }

    [Theory]
    [InlineData("a")]     // 1 character
    [InlineData("ab")]    // 2 characters  
    [InlineData("abc")]   // 3 characters
    [InlineData("abcd")]  // 4 characters
    [InlineData("abcde")] // 5 characters
    public async Task Validator_Should_Fail_On_Password_Too_Short(string password)
    {
        // Given
        var command = CreateValidCommand(password: password);

        // When
        var result = await _validator.ValidateAsync(command);

        // Then
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Password");

        Assert.Contains(result.Errors, e => e.PropertyName == "Password");
    }

    [Fact]
    public async Task Validator_Should_Fail_On_Password_Too_Long()
    {
        // Given - Assuming max length of 128 characters
        var longPassword = new string('a', 129) + "A1!"; // 132 characters total
        var command = CreateValidCommand(password: longPassword);

        // When
        var result = await _validator.ValidateAsync(command);

        // Then
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Password");
    }

    [Theory]
    [InlineData("   ValidPass123!   ")] // Leading/trailing spaces
    [InlineData("Valid Pass123!")]      // Space in middle
    [InlineData("Valid\tPass123!")]     // Tab character
    [InlineData("Valid\nPass123!")]     // Newline character
    public async Task Validator_Should_Fail_On_Password_With_Whitespace(string password)
    {
        // Given
        var command = CreateValidCommand(password: password);

        // When
        var result = await _validator.ValidateAsync(command);

        // Then
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Password");
    }

    [Fact]
    public async Task Validator_Should_Fail_On_Empty_FirstName()
    {
        // Given
        var command = CreateValidCommand(firstName: "");

        // When
        var result = await _validator.ValidateAsync(command);

        // Then
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "FirstName");
    }

    [Fact]
    public async Task Validator_Should_Fail_On_Empty_LastName()
    {
        // Given
        var command = CreateValidCommand(lastName: "");

        // When
        var result = await _validator.ValidateAsync(command);

        // Then
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "LastName");
    }


    [Theory]
    [InlineData(101)]
    [InlineData(1000)]
    public async Task Validator_Should_Fail_On_ExceedMaxLength_FirstName(int FirstNameLength)
    {
        // Given
        var command = CreateValidCommand(firstName: new string('a', FirstNameLength));

        // When
        var result = await _validator.ValidateAsync(command);

        // Then
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "FirstName");
    }

    [Theory]
    [InlineData(101)]
    [InlineData(1000)]
    public async Task Validator_Should_Fail_On_ExceedMaxLength_LastName(int LastNameLength)
    {
        // Given
        var command = CreateValidCommand(lastName: new string('a', LastNameLength));

        // When
        var result = await _validator.ValidateAsync(command);

        // Then
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "LastName");
    }

    [Theory]
    [InlineData("1800-01-01")]
    [InlineData("1850-01-01")]
    public async Task Validator_Should_Fail_On_Very_Old_HireDate(string hireDateString)
    {
        // Given
        var hireDate = DateTime.Parse(hireDateString);
        var command = CreateValidCommand(hireDate: hireDate);

        // When
        var result = await _validator.ValidateAsync(command);

        // Then
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "HireDate");
    }
}