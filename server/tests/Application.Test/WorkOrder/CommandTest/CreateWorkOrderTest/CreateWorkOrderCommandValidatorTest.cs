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
        string assignedToUserId = "guid-1234-5678-9012-345678901234",
        string title = "Test Work Order",
        string? description = "Test Description",
        WorkTypeEnum workOrderType = WorkTypeEnum.SCHEDULED,
        PriorityLevelEnum priorityLevel = PriorityLevelEnum.HIGH,
        WorkOrderStatusEnum status = WorkOrderStatusEnum.ASSIGNED,
        DateTime? scheduledStartDate = null,
        DateTime? actualStartDate = null,
        double startOdometer = 1000.0,
        double? endOdometer = 1010.0,
        List<int>? issueIdList = null,
        List<CreateWorkOrderLineItemDTO>? workOrderLineItems = null
    )
    {
        issueIdList ??= new List<int> { 1, 2, 3 };
        scheduledStartDate ??= DateTime.UtcNow.AddDays(1);
        actualStartDate ??= DateTime.UtcNow.AddDays(2);

        return new CreateWorkOrderCommand(
            vehicleId,
            assignedToUserId,
            title,
            description,
            workOrderType,
            priorityLevel,
            status,
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
    public async Task Validator_Should_Pass_When_StartOdometer_Is_Zero()
    {
        // Given
        var command = CreateValidCommand(startOdometer: 0.0);

        // When
        var result = await _validator.ValidateAsync(command);

        // Then
        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
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
    public async Task Validator_Should_Pass_When_EndOdometer_Equals_StartOdometer()
    {
        // Given
        var command = CreateValidCommand(startOdometer: 1000.0, endOdometer: 1000.0);

        // When
        var result = await _validator.ValidateAsync(command);

        // Then
        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
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
    public async Task Validator_Should_Pass_When_ActualStartDate_Equals_ScheduledStartDate()
    {
        // Given
        var scheduledDate = DateTime.UtcNow.AddDays(1);
        var actualDate = scheduledDate;
        var command = CreateValidCommand(scheduledStartDate: scheduledDate, actualStartDate: actualDate);

        // When
        var result = await _validator.ValidateAsync(command);

        // Then
        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
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
    public async Task Validator_Should_Pass_When_ScheduledStartDate_Is_Within_Buffer()
    {
        // Given - within 5 minute buffer
        var recentDate = DateTime.UtcNow.AddMinutes(-2);
        var command = CreateValidCommand(scheduledStartDate: recentDate);

        // When
        var result = await _validator.ValidateAsync(command);

        // Then
        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }

    [Fact]
    public async Task Validator_Should_Pass_When_Optional_Fields_Are_Null()
    {
        // Given
        var command = CreateValidCommand(
            description: null,
            scheduledStartDate: null,
            actualStartDate: null,
            endOdometer: null,
            issueIdList: null,
            workOrderLineItems: null
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
                UnitPrice = 50.00m,
                Description = "Test item"
            }
        };
        var command = CreateValidCommand(workOrderLineItems: lineItems);

        // When
        var result = await _validator.ValidateAsync(command);

        // Then
        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }

    [Fact]
    public async Task Validator_Should_Pass_When_WorkOrderLineItems_Are_Empty()
    {
        // Given
        var command = CreateValidCommand(workOrderLineItems: new List<CreateWorkOrderLineItemDTO>());

        // When
        var result = await _validator.ValidateAsync(command);

        // Then
        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }

    [Fact]
    public async Task Validator_Should_Pass_With_All_Valid_Enum_Values()
    {
        // Given
        var command = CreateValidCommand(
            workOrderType: WorkTypeEnum.SCHEDULED,
            priorityLevel: PriorityLevelEnum.CRITICAL,
            status: WorkOrderStatusEnum.IN_PROGRESS
        );

        // When
        var result = await _validator.ValidateAsync(command);

        // Then
        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }

    [Fact]
    public async Task Validator_Should_Pass_When_Description_Is_Empty_String()
    {
        // Given
        var command = CreateValidCommand(description: "");

        // When
        var result = await _validator.ValidateAsync(command);

        // Then
        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }

    [Fact]
    public async Task Validator_Should_Pass_When_Description_Is_At_Max_Length()
    {
        // Given
        var command = CreateValidCommand(description: new string('A', 2000)); // Exactly 2000 characters

        // When
        var result = await _validator.ValidateAsync(command);

        // Then
        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }

    [Fact]
    public async Task Validator_Should_Pass_When_Title_Is_At_Max_Length()
    {
        // Given
        var command = CreateValidCommand(title: new string('A', 200)); // Exactly 200 characters

        // When
        var result = await _validator.ValidateAsync(command);

        // Then
        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }
}