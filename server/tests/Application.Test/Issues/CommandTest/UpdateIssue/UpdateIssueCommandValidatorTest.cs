using System;
using System.Linq;
using System.Threading.Tasks;

using Application.Features.Issues.Command.UpdateIssue;

using Domain.Entities.Enums;

using Xunit;

namespace Application.Test.Issues.CommandTest.UpdateIssue;

public class UpdateIssueCommandValidatorTest
{
    private readonly UpdateIssueCommandValidator _validator;

    public UpdateIssueCommandValidatorTest()
    {
        _validator = new UpdateIssueCommandValidator();
    }

    private UpdateIssueCommand CreateValidCommand(
        int issueID = 1,
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
        return new UpdateIssueCommand(issueID, vehicleID, title, description, priorityLevel, category, status, reportedByUserID, reportedDate);
    }

    [Fact]
    public async Task UpdateIssueValidator_Should_Pass_With_Valid_Command()
    {
        var command = CreateValidCommand();
        var result = await _validator.ValidateAsync(command);
        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }

    // ISSUE ID VALIDATION
    [Theory]
    [InlineData(0)]
    public async Task UpdateIssueValidator_Should_Fail_When_IssueID_Is_Empty(int invalidIssueID)
    {
        var command = CreateValidCommand(issueID: invalidIssueID);
        var result = await _validator.ValidateAsync(command);
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "IssueID");
    }

    // VEHICLE ID VALIDATION
    [Theory]
    [InlineData(0)]
    public async Task UpdateIssueValidator_Should_Fail_When_VehicleID_Is_Empty(int invalidVehicleID)
    {
        var command = CreateValidCommand(vehicleID: invalidVehicleID);
        var result = await _validator.ValidateAsync(command);
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "VehicleID");
    }

    // REPORTED BY USER ID VALIDATION
    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public async Task UpdateIssueValidator_Should_Fail_When_ReportedByUserID_Is_Empty(string invalidUserID)
    {
        var command = CreateValidCommand(reportedByUserID: invalidUserID);
        var result = await _validator.ValidateAsync(command);
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "ReportedByUserID");
    }

    // TITLE VALIDATION
    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public async Task UpdateIssueValidator_Should_Fail_When_Title_Is_Empty(string invalidTitle)
    {
        var command = CreateValidCommand(title: invalidTitle);
        var result = await _validator.ValidateAsync(command);
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Title");
    }

    [Theory]
    [InlineData(201)]
    [InlineData(300)]
    public async Task UpdateIssueValidator_Should_Fail_When_Title_Exceeds_MaxLength(int titleLength)
    {
        var command = CreateValidCommand(title: new string('A', titleLength));
        var result = await _validator.ValidateAsync(command);
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Title" && e.ErrorMessage.Contains("200 characters"));
    }

    // DESCRIPTION VALIDATION
    [Theory]
    [InlineData(1001)]
    [InlineData(10000)]
    public async Task UpdateIssueValidator_Should_Fail_When_Description_Exceeds_MaxLength(int descriptionLength)
    {
        var command = CreateValidCommand(description: new string('A', descriptionLength));
        var result = await _validator.ValidateAsync(command);
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Description" && e.ErrorMessage.Contains("1000 characters"));
    }

    // CATEGORY VALIDATION
    [Theory]
    [InlineData(0)]
    [InlineData(10)]
    public async Task UpdateIssueValidator_Should_Fail_When_Category_Is_Invalid(int invalidCategory)
    {
        var command = CreateValidCommand(category: (IssueCategoryEnum)invalidCategory);
        var result = await _validator.ValidateAsync(command);
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Category");
    }

    // PRIORITY LEVEL VALIDATION
    [Theory]
    [InlineData(0)]
    [InlineData(10)]
    public async Task UpdateIssueValidator_Should_Fail_When_PriorityLevel_Is_Invalid(int invalidPriorityLevel)
    {
        var command = CreateValidCommand(priorityLevel: (PriorityLevelEnum)invalidPriorityLevel);
        var result = await _validator.ValidateAsync(command);
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "PriorityLevel");
    }

    // STATUS VALIDATION
    [Theory]
    [InlineData(0)]
    [InlineData(10)]
    public async Task UpdateIssueValidator_Should_Fail_When_Status_Is_Invalid(int invalidStatus)
    {
        var command = CreateValidCommand(status: (IssueStatusEnum)invalidStatus);
        var result = await _validator.ValidateAsync(command);
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Status");
    }
}
