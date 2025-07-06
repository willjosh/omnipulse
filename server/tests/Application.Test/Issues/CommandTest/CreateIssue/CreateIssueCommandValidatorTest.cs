using System;
using System.Linq;

using Application.Features.Issues.Command.CreateIssue;

using Domain.Entities.Enums;

using Xunit;

namespace Application.Test.Issues.CommandTest.CreateIssue;

public class CreateIssueCommandValidatorTest
{
    private readonly CreateIssueCommandValidator _validator;

    public CreateIssueCommandValidatorTest()
    {
        _validator = new CreateIssueCommandValidator();
    }

    private CreateIssueCommand CreateValidCommand(
        int vehicleID = 123,
        string title = "Test Issue Title",
        string? description = "Test Issue Description",
        PriorityLevelEnum priorityLevel = PriorityLevelEnum.CRITICAL,
        IssueCategoryEnum category = IssueCategoryEnum.BODY,
        IssueStatusEnum status = IssueStatusEnum.IN_PROGRESS,
        string reportedByUserID = "1234567890",
        DateTime? reportedDate = null
    )
    {
        return new CreateIssueCommand(vehicleID, title, description, priorityLevel, category, status, reportedByUserID, reportedDate);
    }

    [Fact]
    public async Task CreateIssueValidator_Should_Pass_With_Valid_Command()
    {
        // Given
        var command = CreateValidCommand();

        // When
        var result = await _validator.ValidateAsync(command);

        // Then
        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }

    // TITLE VALIDATION TESTS
    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public async Task CreateIssueValidator_Should_Fail_When_Title_Is_Empty(string invalidTitle)
    {
        // Given
        var command = CreateValidCommand(title: invalidTitle);

        // When
        var result = await _validator.ValidateAsync(command);

        // Then
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Title");
    }

    [Theory]
    [InlineData(201)] // Exceeds 200 limit
    [InlineData(300)] // Way over limit
    public async Task CreateIssueValidator_Should_Fail_When_Title_Exceeds_MaxLength(int titleLength)
    {
        // Given
        var command = CreateValidCommand(title: new string('A', titleLength));

        // When
        var result = await _validator.ValidateAsync(command);

        // Then
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Title" && e.ErrorMessage.Contains("200 characters"));
    }

    // DESCRIPTION VALIDATION TESTS
    [Theory]
    [InlineData(1001)] // Exceeds 1000 limit
    [InlineData(10000)] // Way over limit
    public async Task CreateIssueValidator_Should_Fail_When_Description_Exceeds_MaxLength(int descriptionLength)
    {
        // Given
        var command = CreateValidCommand(description: new string('A', descriptionLength));

        // When
        var result = await _validator.ValidateAsync(command);

        // Then
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Description" && e.ErrorMessage.Contains("1000 characters"));
    }

    // CATEGORY VALIDATION TESTS
    [Theory]
    [InlineData(0)] // Invalid category
    [InlineData(10)] // Invalid category
    public async Task CreateIssueValidator_Should_Fail_When_Category_Is_Invalid(int invalidCategory)
    {
        // Given
        var command = CreateValidCommand(category: (IssueCategoryEnum)invalidCategory);

        // When
        var result = await _validator.ValidateAsync(command);

        // Then
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Category");
    }

    // PRIORITY LEVEL VALIDATION TESTS
    [Theory]
    [InlineData(0)] // Invalid priority level
    [InlineData(10)] // Invalid priority level
    public async Task CreateIssueValidator_Should_Fail_When_PriorityLevel_Is_Invalid(int invalidPriorityLevel)
    {
        // Given
        var command = CreateValidCommand(priorityLevel: (PriorityLevelEnum)invalidPriorityLevel);

        // When
        var result = await _validator.ValidateAsync(command);

        // Then
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "PriorityLevel");
    }

    // STATUS VALIDATION TESTS
    [Theory]
    [InlineData(0)] // Invalid status
    [InlineData(10)] // Invalid status
    public async Task CreateIssueValidator_Should_Fail_When_Status_Is_Invalid(int invalidStatus)
    {
        // Given
        var command = CreateValidCommand(status: (IssueStatusEnum)invalidStatus);

        // When
        var result = await _validator.ValidateAsync(command);

        // Then
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Status");
    }

    // REPORTED BY USER ID VALIDATION TESTS
    [Theory]
    [InlineData("")] // Empty user ID
    [InlineData("   ")] // Whitespace only
    public async Task CreateIssueValidator_Should_Fail_When_ReportedByUserID_Is_Empty(string invalidUserID)
    {
        // Given
        var command = CreateValidCommand(reportedByUserID: invalidUserID);

        // When
        var result = await _validator.ValidateAsync(command);

        // Then
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "ReportedByUserID");
    }
}