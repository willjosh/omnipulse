using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Application.Features.WorkOrderLineItem.Models;
using Application.Features.WorkOrders.Command.UpdateWorkOrder;

using Domain.Entities.Enums;

using FluentValidation;

using Xunit;

namespace Application.Test.WorkOrders.CommandTest.UpdateWorkOrder;

public class UpdateWorkOrderCommandValidatorTest
{
    private readonly UpdateWorkOrderCommandValidator _validator;

    public UpdateWorkOrderCommandValidatorTest()
    {
        _validator = new UpdateWorkOrderCommandValidator();
    }

    private UpdateWorkOrderCommand CreateValidCommand(
        int workOrderId = 1,
        int vehicleId = 1,
        string assignedToUserId = "guid-1234-5678-9012-345678901234",
        string title = "Test Work Order",
        string? description = "Test Description",
        WorkTypeEnum workOrderType = WorkTypeEnum.SCHEDULED,
        PriorityLevelEnum priorityLevel = PriorityLevelEnum.HIGH,
        WorkOrderStatusEnum status = WorkOrderStatusEnum.ASSIGNED,
        DateTime? scheduledStartDate = null,
        DateTime? actualStartDate = null,
        DateTime? scheduledCompletionDate = null,
        DateTime? actualCompletionDate = null,
        double startOdometer = 1000.0,
        double? endOdometer = 1010.0,
        List<int>? issueIdList = null,
        List<CreateWorkOrderLineItemDTO>? workOrderLineItems = null,
        bool setDefaultDates = false
    )
    {
        issueIdList ??= new List<int> { 1, 2, 3 };

        bool needsDefaults = scheduledStartDate == null && actualStartDate == null;

        if (needsDefaults || setDefaultDates)
        {
            scheduledStartDate = DateTime.UtcNow.AddDays(1).AddMinutes(10);
            actualStartDate = DateTime.UtcNow.AddDays(2);
        }

        return new UpdateWorkOrderCommand(
            workOrderId,
            vehicleId,
            assignedToUserId,
            title,
            description,
            workOrderType,
            priorityLevel,
            status,
            scheduledStartDate,
            actualStartDate,
            scheduledCompletionDate,
            actualCompletionDate,
            startOdometer,
            endOdometer,
            issueIdList,
            workOrderLineItems
        );
    }

    [Fact]
    public async Task Validator_Should_Pass_With_Valid_Command()
    {
        var command = CreateValidCommand(setDefaultDates: true);
        var result = await _validator.ValidateAsync(command);
        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }

    [Fact]
    public async Task Validator_Should_Fail_When_WorkOrderId_Is_Zero_Or_Negative()
    {
        var command = CreateValidCommand(workOrderId: 0);
        var result = await _validator.ValidateAsync(command);
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(UpdateWorkOrderCommand.WorkOrderID));
    }

    [Fact]
    public async Task Validator_Should_Fail_When_VehicleId_Is_Zero_Or_Negative()
    {
        var command = CreateValidCommand(vehicleId: 0);
        var result = await _validator.ValidateAsync(command);
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(UpdateWorkOrderCommand.VehicleID));
    }

    [Fact]
    public async Task Validator_Should_Fail_When_AssignedToUserId_Is_Empty()
    {
        var command = CreateValidCommand(assignedToUserId: "");
        var result = await _validator.ValidateAsync(command);
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(UpdateWorkOrderCommand.AssignedToUserID));
    }

    [Fact]
    public async Task Validator_Should_Fail_When_Title_Is_Empty()
    {
        var command = CreateValidCommand(title: "");
        var result = await _validator.ValidateAsync(command);
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(UpdateWorkOrderCommand.Title));
    }

    [Fact]
    public async Task Validator_Should_Fail_When_Title_Exceeds_MaxLength()
    {
        var command = CreateValidCommand(title: new string('A', 201));
        var result = await _validator.ValidateAsync(command);
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(UpdateWorkOrderCommand.Title));
    }

    [Fact]
    public async Task Validator_Should_Fail_When_Description_Exceeds_MaxLength()
    {
        var command = CreateValidCommand(description: new string('A', 2001));
        var result = await _validator.ValidateAsync(command);
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(UpdateWorkOrderCommand.Description));
    }

    [Fact]
    public async Task Validator_Should_Fail_When_StartOdometer_Is_Negative()
    {
        var command = CreateValidCommand(startOdometer: -100.0);
        var result = await _validator.ValidateAsync(command);
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(UpdateWorkOrderCommand.StartOdometer));
    }

    [Fact]
    public async Task Validator_Should_Pass_When_StartOdometer_Is_Zero()
    {
        var command = CreateValidCommand(startOdometer: 0.0);
        var result = await _validator.ValidateAsync(command);
        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }

    [Fact]
    public async Task Validator_Should_Fail_When_EndOdometer_Is_Less_Than_StartOdometer()
    {
        var command = CreateValidCommand(startOdometer: 1000.0, endOdometer: 999.0);
        var result = await _validator.ValidateAsync(command);
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(UpdateWorkOrderCommand.EndOdometer));
    }

    [Fact]
    public async Task Validator_Should_Pass_When_EndOdometer_Equals_StartOdometer()
    {
        var command = CreateValidCommand(startOdometer: 1000.0, endOdometer: 1000.0);
        var result = await _validator.ValidateAsync(command);
        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }

    [Fact]
    public async Task Validator_Should_Fail_When_ActualStartDate_Is_Before_ScheduledStartDate()
    {
        var scheduledDate = DateTime.UtcNow.AddDays(2);
        var actualDate = DateTime.UtcNow.AddDays(1);
        var command = CreateValidCommand(scheduledStartDate: scheduledDate, actualStartDate: actualDate);
        var result = await _validator.ValidateAsync(command);
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(UpdateWorkOrderCommand.ActualStartDate));
    }

    [Fact]
    public async Task Validator_Should_Pass_When_ActualStartDate_Equals_ScheduledStartDate()
    {
        var scheduledDate = DateTime.UtcNow.AddDays(1);
        var actualDate = scheduledDate;
        var command = CreateValidCommand(scheduledStartDate: scheduledDate, actualStartDate: actualDate);
        var result = await _validator.ValidateAsync(command);
        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }

    [Fact]
    public async Task Validator_Should_Fail_When_ScheduledStartDate_Is_In_Past()
    {
        var pastDate = DateTime.UtcNow.AddDays(-1);
        var command = CreateValidCommand(scheduledStartDate: pastDate);
        var result = await _validator.ValidateAsync(command);
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(UpdateWorkOrderCommand.ScheduledStartDate));
    }

    [Fact]
    public async Task Validator_Should_Pass_When_ScheduledStartDate_Is_Within_Buffer()
    {
        var recentDate = DateTime.UtcNow.AddMinutes(-2);
        var command = CreateValidCommand(scheduledStartDate: recentDate);
        var result = await _validator.ValidateAsync(command);
        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }

    [Fact]
    public async Task Validator_Should_Pass_When_Optional_Fields_Are_Null()
    {
        var command = CreateValidCommand(
            description: null,
            scheduledStartDate: null,
            actualStartDate: null,
            endOdometer: null,
            issueIdList: null,
            workOrderLineItems: null
        );
        var result = await _validator.ValidateAsync(command);
        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }

    [Fact]
    public async Task Validator_Should_Pass_When_IssueIdList_Is_Empty()
    {
        var command = CreateValidCommand(issueIdList: new List<int>());
        var result = await _validator.ValidateAsync(command);
        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }

    [Fact]
    public async Task Validator_Should_Pass_When_WorkOrderLineItems_Are_Empty()
    {
        var command = CreateValidCommand(workOrderLineItems: new List<CreateWorkOrderLineItemDTO>());
        var result = await _validator.ValidateAsync(command);
        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }

    [Fact]
    public async Task Validator_Should_Pass_With_All_Valid_Enum_Values()
    {
        var command = CreateValidCommand(
            workOrderType: WorkTypeEnum.SCHEDULED,
            priorityLevel: PriorityLevelEnum.CRITICAL,
            status: WorkOrderStatusEnum.IN_PROGRESS
        );
        var result = await _validator.ValidateAsync(command);
        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }

    [Fact]
    public async Task Validator_Should_Pass_When_Description_Is_Empty_String()
    {
        var command = CreateValidCommand(description: "");
        var result = await _validator.ValidateAsync(command);
        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }

    [Fact]
    public async Task Validator_Should_Pass_When_Description_Is_At_Max_Length()
    {
        var command = CreateValidCommand(description: new string('A', 2000));
        var result = await _validator.ValidateAsync(command);
        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }

    [Fact]
    public async Task Validator_Should_Pass_When_Title_Is_At_Max_Length()
    {
        var command = CreateValidCommand(title: new string('A', 200));
        var result = await _validator.ValidateAsync(command);
        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }

    [Fact]
    public async Task Validator_Should_Fail_When_ScheduledCompletionDate_Is_Before_ScheduledStartDate()
    {
        var scheduledStartDate = DateTime.UtcNow.AddDays(2);
        var scheduledCompletionDate = DateTime.UtcNow.AddDays(1);
        var command = CreateValidCommand(
            scheduledStartDate: scheduledStartDate,
            scheduledCompletionDate: scheduledCompletionDate
        );
        var result = await _validator.ValidateAsync(command);
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(UpdateWorkOrderCommand.ScheduledCompletionDate));
        Assert.Contains(result.Errors, e => e.ErrorMessage == "Scheduled Completion Date must be after Scheduled Start Date");
    }

    [Fact]
    public async Task Validator_Should_Pass_When_ScheduledCompletionDate_Is_After_ScheduledStartDate()
    {
        var scheduledStartDate = DateTime.UtcNow.AddDays(1);
        var scheduledCompletionDate = DateTime.UtcNow.AddDays(3);
        var command = CreateValidCommand(
            scheduledStartDate: scheduledStartDate,
            scheduledCompletionDate: scheduledCompletionDate
        );
        var result = await _validator.ValidateAsync(command);
        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }

    [Fact]
    public async Task Validator_Should_Pass_When_ScheduledCompletionDate_Is_Null()
    {
        var command = CreateValidCommand(scheduledCompletionDate: null);
        var result = await _validator.ValidateAsync(command);
        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }

    [Fact]
    public async Task Validator_Should_Fail_When_ActualCompletionDate_Is_Before_ActualStartDate()
    {
        var actualStartDate = DateTime.UtcNow.AddDays(2);
        var actualCompletionDate = DateTime.UtcNow.AddDays(1);
        var command = CreateValidCommand(
            actualStartDate: actualStartDate,
            actualCompletionDate: actualCompletionDate
        );
        var result = await _validator.ValidateAsync(command);
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(UpdateWorkOrderCommand.ActualCompletionDate));
        Assert.Contains(result.Errors, e => e.ErrorMessage == "Actual Completion Date must be after Actual Start Date");
    }

    [Fact]
    public async Task Validator_Should_Pass_When_ActualCompletionDate_Is_After_ActualStartDate()
    {
        var actualStartDate = DateTime.UtcNow.AddDays(1);
        var actualCompletionDate = DateTime.UtcNow.AddDays(3);
        var command = CreateValidCommand(
            actualStartDate: actualStartDate,
            actualCompletionDate: actualCompletionDate
        );
        var result = await _validator.ValidateAsync(command);
        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }

    [Fact]
    public async Task Validator_Should_Pass_When_ActualCompletionDate_Is_Null()
    {
        var command = CreateValidCommand(actualCompletionDate: null);
        var result = await _validator.ValidateAsync(command);
        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }

    [Fact]
    public async Task Validator_Should_Fail_When_ActualCompletionDate_Is_Before_ScheduledCompletionDate()
    {
        var scheduledCompletionDate = DateTime.UtcNow.AddDays(3);
        var actualCompletionDate = DateTime.UtcNow.AddDays(2);
        var command = CreateValidCommand(
            scheduledCompletionDate: scheduledCompletionDate,
            actualCompletionDate: actualCompletionDate
        );
        var result = await _validator.ValidateAsync(command);
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(UpdateWorkOrderCommand.ActualCompletionDate));
        Assert.Contains(result.Errors, e => e.ErrorMessage == "Actual Completion Date must be greater than or equal to Scheduled Completion Date");
    }

    [Fact]
    public async Task Validator_Should_Pass_When_ActualCompletionDate_Equals_ScheduledCompletionDate()
    {
        var completionDate = DateTime.UtcNow.AddDays(3);
        var command = CreateValidCommand(
            scheduledCompletionDate: completionDate,
            actualCompletionDate: completionDate
        );
        var result = await _validator.ValidateAsync(command);
        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }

    [Fact]
    public async Task Validator_Should_Pass_When_ActualCompletionDate_Is_After_ScheduledCompletionDate()
    {
        var scheduledCompletionDate = DateTime.UtcNow.AddDays(2);
        var actualCompletionDate = DateTime.UtcNow.AddDays(4);
        var command = CreateValidCommand(
            scheduledCompletionDate: scheduledCompletionDate,
            actualCompletionDate: actualCompletionDate
        );
        var result = await _validator.ValidateAsync(command);
        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }

    [Fact]
    public async Task Validator_Should_Pass_With_Complete_Valid_Date_Chain()
    {
        var scheduledStart = DateTime.UtcNow.AddDays(1);
        var actualStart = DateTime.UtcNow.AddDays(2);
        var scheduledCompletion = DateTime.UtcNow.AddDays(3);
        var actualCompletion = DateTime.UtcNow.AddDays(4);

        var command = CreateValidCommand(
            scheduledStartDate: scheduledStart,
            actualStartDate: actualStart,
            scheduledCompletionDate: scheduledCompletion,
            actualCompletionDate: actualCompletion
        );
        var result = await _validator.ValidateAsync(command);
        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }

    [Fact]
    public async Task Validator_Should_Fail_With_Multiple_Date_Validation_Errors()
    {
        var scheduledStart = DateTime.UtcNow.AddDays(3);
        var actualStart = DateTime.UtcNow.AddDays(1);
        var scheduledCompletion = DateTime.UtcNow.AddDays(2);
        var actualCompletion = DateTime.UtcNow.AddDays(1);

        var command = CreateValidCommand(
            scheduledStartDate: scheduledStart,
            actualStartDate: actualStart,
            scheduledCompletionDate: scheduledCompletion,
            actualCompletionDate: actualCompletion
        );
        var result = await _validator.ValidateAsync(command);
        Assert.False(result.IsValid);
        Assert.True(result.Errors.Count >= 3);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(UpdateWorkOrderCommand.ActualStartDate));
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(UpdateWorkOrderCommand.ScheduledCompletionDate));
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(UpdateWorkOrderCommand.ActualCompletionDate));
    }
}