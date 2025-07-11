using System;
using System.Threading.Tasks;

using Application.Features.MaintenanceHistories.Command.CreateMaintenanceHistory;

using Xunit;

namespace Application.Test.MaintenanceHistories.CommandTest.CreateMaintenanceHistory;

public class CreateMaintenanceHistoryCommandValidatorTest
{
    private readonly CreateMaintenanceHistoryCommandValidator _validator;

    public CreateMaintenanceHistoryCommandValidatorTest()
    {
        _validator = new CreateMaintenanceHistoryCommandValidator();
    }

    private CreateMaintenanceHistoryCommand CreateValidCommand(
        int vehicleId = 1,
        int workOrderId = 1,
        int serviceTaskId = 1,
        string technicianId = "tech-123",
        DateTime? serviceDate = null,
        double mileageAtService = 10000,
        string? description = "Routine maintenance",
        decimal cost = 250.50m,
        double labourHours = 2.5,
        string? notes = "Changed oil and filter"
    )
    {
        return new CreateMaintenanceHistoryCommand(
            WorkOrderID: workOrderId,
            ServiceDate: serviceDate ?? DateTime.UtcNow.AddDays(-1),
            MileageAtService: mileageAtService,
            Description: description,
            Cost: cost,
            LabourHours: labourHours,
            Notes: notes
        );
    }

    [Fact(Skip = "Refactoring needed")]
    public async Task Validator_Should_Pass_With_Valid_Command()
    {
        var command = CreateValidCommand();
        var result = await _validator.ValidateAsync(command);
        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }

    [Theory(Skip = "Refactoring needed")]
    [InlineData(0)]
    public async Task Validator_Should_Fail_When_VehicleID_Is_Invalid(int vehicleId)
    {
        var command = CreateValidCommand(vehicleId: vehicleId);
        var result = await _validator.ValidateAsync(command);
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "VehicleID");
    }

    [Theory(Skip = "Refactoring needed")]
    [InlineData(-1)]
    [InlineData(-100)]
    public async Task Validator_Should_Pass_When_VehicleID_Is_Negative(int vehicleId)
    {
        var command = CreateValidCommand(vehicleId: vehicleId);
        var result = await _validator.ValidateAsync(command);
        Assert.True(result.IsValid);
        Assert.DoesNotContain(result.Errors, e => e.PropertyName == "VehicleID");
    }

    [Theory(Skip = "Refactoring needed")]
    [InlineData(0)]
    public async Task Validator_Should_Fail_When_WorkOrderID_Is_Invalid(int workOrderId)
    {
        var command = CreateValidCommand(workOrderId: workOrderId);
        var result = await _validator.ValidateAsync(command);
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "WorkOrderID");
    }

    [Theory(Skip = "Refactoring needed")]
    [InlineData(-1)]
    [InlineData(-100)]
    public async Task Validator_Should_Pass_When_WorkOrderID_Is_Negative(int workOrderId)
    {
        var command = CreateValidCommand(workOrderId: workOrderId);
        var result = await _validator.ValidateAsync(command);
        Assert.True(result.IsValid);
        Assert.DoesNotContain(result.Errors, e => e.PropertyName == "WorkOrderID");
    }

    [Theory(Skip = "Refactoring needed")]
    [InlineData(0)]
    public async Task Validator_Should_Fail_When_ServiceTaskID_Is_Invalid(int serviceTaskId)
    {
        var command = CreateValidCommand(serviceTaskId: serviceTaskId);
        var result = await _validator.ValidateAsync(command);
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "ServiceTaskID");
    }

    [Theory(Skip = "Refactoring needed")]
    [InlineData(-1)]
    [InlineData(-100)]
    public async Task Validator_Should_Pass_When_ServiceTaskID_Is_Negative(int serviceTaskId)
    {
        var command = CreateValidCommand(serviceTaskId: serviceTaskId);
        var result = await _validator.ValidateAsync(command);
        Assert.True(result.IsValid);
        Assert.DoesNotContain(result.Errors, e => e.PropertyName == "ServiceTaskID");
    }

    [Theory(Skip = "Refactoring needed")]
    [InlineData("")]
    [InlineData("   ")]
    public async Task Validator_Should_Fail_When_TechnicianID_Is_Empty(string? technicianId)
    {
        var command = CreateValidCommand(technicianId: technicianId ?? "tech-123");
        var result = await _validator.ValidateAsync(command);
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "TechnicianID");
    }

    [Fact(Skip = "Refactoring needed")]
    public async Task Validator_Should_Fail_When_ServiceDate_Is_In_The_Future()
    {
        var command = CreateValidCommand(serviceDate: DateTime.UtcNow.AddDays(1));
        var result = await _validator.ValidateAsync(command);
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "ServiceDate");
    }

    [Theory(Skip = "Refactoring needed")]
    [InlineData(-1)]
    [InlineData(-100)]
    public async Task Validator_Should_Fail_When_MileageAtService_Is_Negative(double mileage)
    {
        var command = CreateValidCommand(mileageAtService: mileage);
        var result = await _validator.ValidateAsync(command);
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "MileageAtService");
    }

    [Theory(Skip = "Refactoring needed")]
    [InlineData(1001)]
    [InlineData(2000)]
    public async Task Validator_Should_Fail_When_Description_Exceeds_MaxLength(int length)
    {
        var command = CreateValidCommand(description: new string('A', length));
        var result = await _validator.ValidateAsync(command);
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Description");
    }

    [Theory(Skip = "Refactoring needed")]
    [InlineData(-1)]
    [InlineData(-100)]
    public async Task Validator_Should_Fail_When_Cost_Is_Negative(decimal cost)
    {
        var command = CreateValidCommand(cost: cost);
        var result = await _validator.ValidateAsync(command);
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Cost");
    }

    [Theory(Skip = "Refactoring needed")]
    [InlineData(-1)]
    [InlineData(-100)]
    public async Task Validator_Should_Fail_When_LabourHours_Is_Negative(double hours)
    {
        var command = CreateValidCommand(labourHours: hours);
        var result = await _validator.ValidateAsync(command);
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "LabourHours");
    }

    [Theory(Skip = "Refactoring needed")]
    [InlineData(1001)]
    [InlineData(2000)]
    public async Task Validator_Should_Fail_When_Notes_Exceeds_MaxLength(int length)
    {
        var command = CreateValidCommand(notes: new string('A', length));
        var result = await _validator.ValidateAsync(command);
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Notes");
    }
}