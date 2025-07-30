using Application.Features.Issues.Command.UpdateIssue;

using Domain.Entities.Enums;

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
        DateTime? reportedDate = null,
        string? resolutionNotes = null,
        DateTime? resolvedDate = null,
        string? resolvedByUserID = null
    )
    {
        return new UpdateIssueCommand(issueID, vehicleID, title, description, priorityLevel, category, status, reportedByUserID, reportedDate, resolutionNotes, resolvedDate, resolvedByUserID);
    }

    [Fact]
    public async Task UpdateIssueValidator_Should_Pass_With_Valid_Command()
    {
        var command = CreateValidCommand(resolutionNotes: "Resolved", resolvedDate: DateTime.UtcNow, resolvedByUserID: "user2");
        var result = await _validator.ValidateAsync(command);
        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }

    // ISSUE ID VALIDATION
    [Theory]
    [InlineData(0)]
    public async Task UpdateIssueValidator_Should_Fail_When_IssueID_Is_Empty(int invalidIssueID)
    {
        var command = CreateValidCommand(issueID: invalidIssueID, resolutionNotes: null, resolvedDate: null, resolvedByUserID: null);
        var result = await _validator.ValidateAsync(command);
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "IssueID");
    }

    // VEHICLE ID VALIDATION
    [Theory]
    [InlineData(0)]
    public async Task UpdateIssueValidator_Should_Fail_When_VehicleID_Is_Empty(int invalidVehicleID)
    {
        var command = CreateValidCommand(vehicleID: invalidVehicleID, resolutionNotes: null, resolvedDate: null, resolvedByUserID: null);
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
        var command = CreateValidCommand(reportedByUserID: invalidUserID, resolutionNotes: null, resolvedDate: null, resolvedByUserID: null);
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
        var command = CreateValidCommand(title: invalidTitle, resolutionNotes: null, resolvedDate: null, resolvedByUserID: null);
        var result = await _validator.ValidateAsync(command);
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Title");
    }

    [Theory]
    [InlineData(201)]
    [InlineData(300)]
    public async Task UpdateIssueValidator_Should_Fail_When_Title_Exceeds_MaxLength(int titleLength)
    {
        var command = CreateValidCommand(title: new string('A', titleLength), resolutionNotes: null, resolvedDate: null, resolvedByUserID: null);
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
        var command = CreateValidCommand(description: new string('A', descriptionLength), resolutionNotes: null, resolvedDate: null, resolvedByUserID: null);
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
        var command = CreateValidCommand(category: (IssueCategoryEnum)invalidCategory, resolutionNotes: null, resolvedDate: null, resolvedByUserID: null);
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
        var command = CreateValidCommand(priorityLevel: (PriorityLevelEnum)invalidPriorityLevel, resolutionNotes: null, resolvedDate: null, resolvedByUserID: null);
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
        var command = CreateValidCommand(status: (IssueStatusEnum)invalidStatus, resolutionNotes: null, resolvedDate: null, resolvedByUserID: null);
        var result = await _validator.ValidateAsync(command);
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Status");
    }

    // CONDITIONAL VALIDATION FOR RESOLVED STATUS
    [Fact]
    public async Task UpdateIssueValidator_Should_Fail_When_ResolutionNotes_Is_Empty_And_Status_Is_Resolved()
    {
        var command = CreateValidCommand(status: IssueStatusEnum.RESOLVED, resolutionNotes: null, resolvedDate: DateTime.UtcNow, resolvedByUserID: "user2");
        var result = await _validator.ValidateAsync(command);
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "ResolutionNotes" && e.ErrorMessage.Contains("Resolution notes are required"));
    }

    [Fact]
    public async Task UpdateIssueValidator_Should_Fail_When_ResolvedDate_Is_Empty_And_Status_Is_Resolved()
    {
        var command = CreateValidCommand(status: IssueStatusEnum.RESOLVED, resolutionNotes: "Resolved", resolvedDate: null, resolvedByUserID: "user2");
        var result = await _validator.ValidateAsync(command);
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "ResolvedDate" && e.ErrorMessage.Contains("Resolved date is required"));
    }

    [Fact]
    public async Task UpdateIssueValidator_Should_Fail_When_ResolvedByUserID_Is_Empty_And_Status_Is_Resolved()
    {
        var command = CreateValidCommand(status: IssueStatusEnum.RESOLVED, resolutionNotes: "Resolved", resolvedDate: DateTime.UtcNow, resolvedByUserID: null);
        var result = await _validator.ValidateAsync(command);
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "ResolvedByUserID" && e.ErrorMessage.Contains("Resolved by user ID is required"));
    }

    [Fact]
    public async Task UpdateIssueValidator_Should_Pass_When_All_Resolved_Fields_Are_Present_And_Status_Is_Resolved()
    {
        var command = CreateValidCommand(status: IssueStatusEnum.RESOLVED, resolutionNotes: "Resolved", resolvedDate: DateTime.UtcNow, resolvedByUserID: "user2");
        var result = await _validator.ValidateAsync(command);
        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }
}