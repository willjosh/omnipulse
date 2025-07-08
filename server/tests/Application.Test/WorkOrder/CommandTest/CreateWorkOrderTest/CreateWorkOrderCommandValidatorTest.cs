using System;

using Application.Features.WorkOrderLineItem.Models;
using Application.Features.WorkOrders.Command.CreateWorkOrder;

using Domain.Entities.Enums;

namespace Application.Test.WorkOrder.CommandTest.CreateWorkOrderTest;

public class CreateWorkOrderCommandValidatorTest
{
    private readonly CreateWorkOrderCommandValidator _validator;

    public CreateWorkOrderCommandValidatorTest()
    {
        _validator = new CreateWorkOrderCommandValidator();
    }

    private CreateWorkOrderCommand CreateValidCommand(
        int vehicleId = 1,
        int serviceReminderId = 1,
        string assignedToUserId = "guid-1234-5678-9012-345678901234",
        string title = "Test Work Order",
        string? description = "Test Description",
        WorkTypeEnum workOrderType = WorkTypeEnum.SCHEDULED,
        PriorityLevelEnum priorityLevel = PriorityLevelEnum.CRITICAL,
        WorkOrderStatusEnum status = WorkOrderStatusEnum.ASSIGNED,
        decimal? estimatedCost = 100.00m,
        decimal? actualCost = 80.00m,
        double? estimatedHours = 2.5,
        double? actualHours = 2.0,
        DateTime? scheduledStartDate = null,
        DateTime? actualStartDate = null,
        double startOdometer = 1000.0,
        double? endOdometer = 1010.0,
        List<int>? issueIdList = null,
        List<CreateWorkOrderLineItemDTO>? workOrderLineItems = null
    )
    {

        issueIdList ??= new List<int>([1, 2, 3]);
        scheduledStartDate ??= DateTime.UtcNow.AddDays(1);
        actualStartDate ??= DateTime.UtcNow.AddDays(2);


        return new CreateWorkOrderCommand(
            vehicleId,
            serviceReminderId,
            assignedToUserId,
            title,
            description,
            workOrderType,
            priorityLevel,
            status,
            estimatedCost,
            actualCost,
            estimatedHours,
            actualHours,
            scheduledStartDate,
            actualStartDate,
            startOdometer,
            endOdometer,
            issueIdList,
            workOrderLineItems
        );
    }

    [Fact]
    public async Task Validator_Should_Pass_With_Valid_Command()
    {
        // Given
        var command = CreateValidCommand();

        // When
        var result = await _validator.ValidateAsync(command);

        // Then
        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }

    [Fact]
    public async Task Validator_Should_Fail_When_AssignedToUserId_Is_Empty()
    {
        // Given
        var command = CreateValidCommand(assignedToUserId: "");

        // When
        var result = await _validator.ValidateAsync(command);

        // Then
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(CreateWorkOrderCommand.AssignedToUserID));
    }

    [Fact]
    public async Task Validator_Should_Fail_When_Title_Is_Empty()
    {
        // Given
        var command = CreateValidCommand(title: "");

        // When
        var result = await _validator.ValidateAsync(command);

        // Then
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(CreateWorkOrderCommand.Title));
    }

    [Fact]
    public async Task Validator_Should_Fail_When_Title_Exceeds_MaxLength()
    {
        // Given
        var command = CreateValidCommand(title: new string('A', 201)); // 201 characters

        // When
        var result = await _validator.ValidateAsync(command);

        // Then
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(CreateWorkOrderCommand.Title));
    }

    [Fact]
    public async Task Validator_Should_Fail_When_Description_Exceeds_MaxLength()
    {
        // Given
        var command = CreateValidCommand(description: new string('A', 2001)); // 2001 characters

        // When
        var result = await _validator.ValidateAsync(command);

        // Then
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(CreateWorkOrderCommand.Description));
    }

    [Fact]
    public async Task Validator_Should_Fail_When_EstimatedCost_Is_Negative()
    {
        // Given
        var command = CreateValidCommand(estimatedCost: -10.00m);

        // When
        var result = await _validator.ValidateAsync(command);

        // Then
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(CreateWorkOrderCommand.EstimatedCost));
    }

    [Fact]
    public async Task Validator_Should_Fail_When_EstimatedHours_Is_Zero_Or_Negative()
    {
        // Given
        var command = CreateValidCommand(estimatedHours: 0);

        // When
        var result = await _validator.ValidateAsync(command);

        // Then
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(CreateWorkOrderCommand.EstimatedHours));
    }

    [Fact]
    public async Task Validator_Should_Fail_When_StartOdometer_Is_Negative()
    {
        // Given
        var command = CreateValidCommand(startOdometer: -100.0);

        // When
        var result = await _validator.ValidateAsync(command);

        // Then
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(CreateWorkOrderCommand.StartOdometer));
    }

    [Fact]
    public async Task Validator_Should_Fail_When_EndOdometer_Is_Less_Than_StartOdometer()
    {
        // Given
        var command = CreateValidCommand(startOdometer: 1000.0, endOdometer: 999.0);

        // When
        var result = await _validator.ValidateAsync(command);

        // Then
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(CreateWorkOrderCommand.EndOdometer));
    }

    [Fact]
    public async Task Validator_Should_Fail_When_ActualStartDate_Is_Before_ScheduledStartDate()
    {
        // Given
        var scheduledDate = DateTime.UtcNow.AddDays(2);
        var actualDate = DateTime.UtcNow.AddDays(1);
        var command = CreateValidCommand(scheduledStartDate: scheduledDate, actualStartDate: actualDate);

        // When
        var result = await _validator.ValidateAsync(command);

        // Then
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(CreateWorkOrderCommand.ActualStartDate));
    }

    [Fact]
    public async Task Validator_Should_Fail_When_ScheduledStartDate_Is_In_Past()
    {
        // Given
        var pastDate = DateTime.UtcNow.AddDays(-1);
        var command = CreateValidCommand(scheduledStartDate: pastDate);

        // When
        var result = await _validator.ValidateAsync(command);

        // Then
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(CreateWorkOrderCommand.ScheduledStartDate));
    }

    [Fact]
    public async Task Validator_Should_Pass_When_Optional_Fields_Are_Null()
    {
        // Given
        var command = CreateValidCommand(
            description: null,
            estimatedCost: null,
            actualCost: null,
            estimatedHours: null,
            actualHours: null,
            scheduledStartDate: null,
            actualStartDate: null,
            endOdometer: null,
            issueIdList: null
        );

        // When
        var result = await _validator.ValidateAsync(command);

        // Then
        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }

    [Fact]
    public async Task Validator_Should_Pass_When_IssueIdList_Is_Empty()
    {
        // Given
        var command = CreateValidCommand(issueIdList: new List<int>());

        // When
        var result = await _validator.ValidateAsync(command);

        // Then
        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }

    [Fact]
    public async Task Validator_Should_Fail_When_ActualCost_Is_Negative()
    {
        // Given
        var command = CreateValidCommand(actualCost: -10.00m);

        // When
        var result = await _validator.ValidateAsync(command);

        // Then
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(CreateWorkOrderCommand.ActualCost));
    }

    [Fact]
    public async Task Validator_Should_Fail_When_ActualCost_Has_Too_Many_Decimal_Places()
    {
        // Given
        var command = CreateValidCommand(actualCost: 123.456m); // 3 decimal places

        // When
        var result = await _validator.ValidateAsync(command);

        // Then
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(CreateWorkOrderCommand.ActualCost));
    }

    [Fact]
    public async Task Validator_Should_Fail_When_ActualCost_Has_Too_Many_Total_Digits()
    {
        // Given
        var command = CreateValidCommand(actualCost: 12345678901m); // 11 total digits

        // When
        var result = await _validator.ValidateAsync(command);

        // Then
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(CreateWorkOrderCommand.ActualCost));
    }

    [Fact]
    public async Task Validator_Should_Fail_When_ActualHours_Is_Zero_Or_Negative()
    {
        // Given
        var command = CreateValidCommand(actualHours: 0);

        // When
        var result = await _validator.ValidateAsync(command);

        // Then
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(CreateWorkOrderCommand.ActualHours));
    }

    [Fact]
    public async Task Validator_Should_Fail_When_ActualHours_Is_Negative()
    {
        // Given
        var command = CreateValidCommand(actualHours: -1.5);

        // When
        var result = await _validator.ValidateAsync(command);

        // Then
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(CreateWorkOrderCommand.ActualHours));
    }

    [Fact]
    public async Task Validator_Should_Fail_When_VehicleId_Is_Zero_Or_Negative()
    {
        // Given
        var command = CreateValidCommand(vehicleId: 0);

        // When
        var result = await _validator.ValidateAsync(command);

        // Then
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(CreateWorkOrderCommand.VehicleID));
    }

    [Fact]
    public async Task Validator_Should_Fail_When_ServiceReminderId_Is_Zero_Or_Negative()
    {
        // Given
        var command = CreateValidCommand(serviceReminderId: -1);

        // When
        var result = await _validator.ValidateAsync(command);

        // Then
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(CreateWorkOrderCommand.ServiceReminderID));
    }

    [Fact]
    public async Task Validator_Should_Pass_When_ActualFields_Are_Null()
    {
        // Given
        var command = CreateValidCommand(
            actualCost: null,
            actualHours: null
        );

        // When
        var result = await _validator.ValidateAsync(command);

        // Then
        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }

    [Fact]
    public async Task Validator_Should_Pass_When_ActualFields_Are_Valid()
    {
        // Given
        var command = CreateValidCommand(
            actualCost: 99.99m,
            actualHours: 3.5
        );

        // When
        var result = await _validator.ValidateAsync(command);

        // Then
        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }

    [Fact]
    public async Task Validator_Should_Pass_When_WorkOrderLineItems_Are_Valid()
    {
        // Given
        var lineItems = new List<CreateWorkOrderLineItemDTO>
        {
            new CreateWorkOrderLineItemDTO
            {
                ServiceTaskID = 1,
                InventoryItemID = 1,
                ItemType = LineItemTypeEnum.ITEM,
                Quantity = 1,
                UnitPrice = 50.00m
            }
        };
        var command = CreateValidCommand(workOrderLineItems: lineItems);

        // When
        var result = await _validator.ValidateAsync(command);

        // Then
        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }
}