using System;

using Application.Features.Users.Command.UpdateTechnician;

using FluentValidation;

namespace Application.Test.Users.CommandTest.UpdateTechnicianTest;

public class UpdateTechnicianValidatorTest
{
    public readonly IValidator<UpdateTechnicianCommand> _validator;

    public UpdateTechnicianValidatorTest()
    {
        _validator = new UpdateTechnicianCommandValidator();
    }

    private UpdateTechnicianCommand CreateValidCommand(
        string id = "",
        string email = "ClarenceGanteng@gmail.com",
        string firstName = "Clarence",
        string lastName = "Muljadi",
        DateTime? hireDate = null,
        bool isActive = true
    )
    {
        return new UpdateTechnicianCommand(
            Id: Guid.NewGuid().ToString(),
            firstName,
            lastName,
            hireDate ?? DateTime.UtcNow,
            isActive
        );
    }

    [Fact(Skip = "Not implemented yet")]
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

    [Theory(Skip = "Not implemented yet")]
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

    [Theory(Skip = "Not implemented yet")]
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

    [Theory(Skip = "Not implemented yet")]
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

    [Theory(Skip = "Not implemented yet")]
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