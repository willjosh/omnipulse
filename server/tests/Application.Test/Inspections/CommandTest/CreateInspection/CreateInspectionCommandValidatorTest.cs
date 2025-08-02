using Application.Features.Inspections.Command.CreateInspection;

using Domain.Entities.Enums;

namespace Application.Test.Inspections.CommandTest.CreateInspection;

public class CreateInspectionCommandValidatorTest
{
    private readonly CreateInspectionCommandValidator _validator;

    public CreateInspectionCommandValidatorTest()
    {
        _validator = new CreateInspectionCommandValidator();
    }

    private const int NotesMaxLength = 2000;

    private static CreateInspectionCommand CreateValidCommand(
        int inspectionFormId = 1,
        int vehicleId = 1,
        string technicianId = "1b6ed760-b26c-4800-8e86-c888d002b9c1",
        DateTime? inspectionStartTime = null,
        DateTime? inspectionEndTime = null,
        double? odometerReading = 50000.0,
        VehicleConditionEnum vehicleCondition = VehicleConditionEnum.Excellent,
        string? notes = "Test inspection notes",
        List<CreateInspectionPassFailItemCommand>? inspectionItems = null)
    {
        var pastTime = DateTime.UtcNow.AddHours(-2);
        var startTime = inspectionStartTime ?? pastTime;
        var endTime = inspectionEndTime ?? pastTime.AddHours(1);
        var items = inspectionItems ??
        [
            new(1, true, "Test comment")
        ];

        return new CreateInspectionCommand(
            inspectionFormId,
            vehicleId,
            technicianId,
            startTime,
            endTime,
            odometerReading,
            vehicleCondition,
            notes,
            items
        );
    }

    [Fact]
    public void Validate_Should_Pass_For_Valid_Command()
    {
        // Arrange
        var command = CreateValidCommand();

        // Act
        var result = _validator.Validate(command);

        // Assert
        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Validate_Should_Fail_When_InspectionFormID_Is_Invalid(int invalidId)
    {
        // Arrange
        var command = CreateValidCommand(inspectionFormId: invalidId);

        // Act
        var result = _validator.Validate(command);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(CreateInspectionCommand.InspectionFormID));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Validate_Should_Fail_When_VehicleID_Is_Invalid(int invalidId)
    {
        // Arrange
        var command = CreateValidCommand(vehicleId: invalidId);

        // Act
        var result = _validator.Validate(command);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(CreateInspectionCommand.VehicleID));
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Validate_Should_Fail_When_TechnicianID_Is_Empty_Or_Null(string? invalidTechnicianId)
    {
        // Arrange
        var command = CreateValidCommand(technicianId: invalidTechnicianId!);

        // Act
        var result = _validator.Validate(command);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(CreateInspectionCommand.TechnicianID));
    }

    [Fact]
    public void Validate_Should_Fail_When_InspectionStartTime_Is_Default()
    {
        // Arrange
        var pastTime = DateTime.UtcNow.AddHours(-2);
        var command = new CreateInspectionCommand(
            1, 1, "1b6ed760-b26c-4800-8e86-c888d002b9c1", default, pastTime.AddHours(1), 50000.0, VehicleConditionEnum.Excellent, "Test notes",
            [new(1, true, "Test comment")]);

        // Act
        var result = _validator.Validate(command);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(CreateInspectionCommand.InspectionStartTime));
    }

    [Fact]
    public void Validate_Should_Fail_When_InspectionStartTime_Is_In_Future()
    {
        // Arrange
        var futureTime = DateTime.UtcNow.AddHours(1);
        var command = CreateValidCommand(inspectionStartTime: futureTime);

        // Act
        var result = _validator.Validate(command);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(CreateInspectionCommand.InspectionStartTime));
    }

    [Fact]
    public void Validate_Should_Fail_When_InspectionEndTime_Is_Default()
    {
        // Arrange
        var pastTime = DateTime.UtcNow.AddHours(-2);
        var command = new CreateInspectionCommand(
            1, 1, "1b6ed760-b26c-4800-8e86-c888d002b9c1", pastTime, default, 50000.0, VehicleConditionEnum.Excellent, "Test notes",
            [new(1, true, "Test comment")]);

        // Act
        var result = _validator.Validate(command);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(CreateInspectionCommand.InspectionEndTime));
    }

    [Fact]
    public void Validate_Should_Fail_When_InspectionEndTime_Is_In_Future()
    {
        // Arrange
        var futureTime = DateTime.UtcNow.AddHours(1);
        var command = CreateValidCommand(inspectionEndTime: futureTime);

        // Act
        var result = _validator.Validate(command);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(CreateInspectionCommand.InspectionEndTime));
    }

    [Fact]
    public void Validate_Should_Fail_When_EndTime_Is_Before_StartTime()
    {
        // Arrange
        var startTime = DateTime.UtcNow.AddHours(-2);
        var endTime = startTime.AddHours(-1);
        var command = CreateValidCommand(
            inspectionStartTime: startTime,
            inspectionEndTime: endTime);

        // Act
        var result = _validator.Validate(command);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "");
    }

    [Fact]
    public void Validate_Should_Pass_When_EndTime_Equals_StartTime()
    {
        // Arrange
        var pastTime = DateTime.UtcNow.AddHours(-2);
        var command = CreateValidCommand(
            inspectionStartTime: pastTime,
            inspectionEndTime: pastTime);

        // Act
        var result = _validator.Validate(command);

        // Assert
        Assert.True(result.IsValid);
    }

    [Theory]
    [InlineData(-1.0)]
    [InlineData(-100.0)]
    public void Validate_Should_Fail_When_OdometerReading_Is_Negative(double negativeValue)
    {
        // Arrange
        var command = CreateValidCommand(odometerReading: negativeValue);

        // Act
        var result = _validator.Validate(command);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(CreateInspectionCommand.OdometerReading));
    }

    [Fact]
    public void Validate_Should_Pass_When_OdometerReading_Is_Zero()
    {
        // Arrange
        var command = CreateValidCommand(odometerReading: 0);

        // Act
        var result = _validator.Validate(command);

        // Assert
        Assert.True(result.IsValid);
    }

    [Fact]
    public void Validate_Should_Pass_When_OdometerReading_Is_Null()
    {
        // Arrange
        var command = CreateValidCommand(odometerReading: null);

        // Act
        var result = _validator.Validate(command);

        // Assert
        Assert.True(result.IsValid);
    }

    [Fact]
    public void Validate_Should_Fail_When_VehicleCondition_Is_Invalid()
    {
        // Arrange
        var invalidCondition = (VehicleConditionEnum)999;
        var command = CreateValidCommand(vehicleCondition: invalidCondition);

        // Act
        var result = _validator.Validate(command);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(CreateInspectionCommand.VehicleCondition));
    }

    [Fact]
    public void Validate_Should_Pass_When_Notes_Is_Null()
    {
        // Arrange
        var command = CreateValidCommand(notes: null);

        // Act
        var result = _validator.Validate(command);

        // Assert
        Assert.True(result.IsValid);
    }

    [Fact]
    public void Validate_Should_Pass_When_Notes_Is_Empty()
    {
        // Arrange
        var command = CreateValidCommand(notes: "");

        // Act
        var result = _validator.Validate(command);

        // Assert
        Assert.True(result.IsValid);
    }

    [Fact]
    public void Validate_Should_Fail_When_Notes_Exceeds_Maximum_Length()
    {
        // Arrange
        var longNotes = new string('a', NotesMaxLength + 1);
        var command = CreateValidCommand(notes: longNotes);

        // Act
        var result = _validator.Validate(command);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(CreateInspectionCommand.Notes));
    }

    [Fact]
    public void Validate_Should_Pass_When_Notes_Is_At_Maximum_Length()
    {
        // Arrange
        var notesAtMaxLength = new string('a', NotesMaxLength);
        var command = CreateValidCommand(notes: notesAtMaxLength);

        // Act
        var result = _validator.Validate(command);

        // Assert
        Assert.True(result.IsValid);
    }

    [Fact]
    public void Validate_Should_Fail_When_InspectionItems_Is_Null()
    {
        // Arrange
        var pastTime = DateTime.UtcNow.AddHours(-2);
        var command = new CreateInspectionCommand(
            1, 1, "1b6ed760-b26c-4800-8e86-c888d002b9c1", pastTime, pastTime.AddHours(1), 50000.0, VehicleConditionEnum.Excellent, "Test notes",
            null!);

        // Act
        var result = _validator.Validate(command);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(CreateInspectionCommand.InspectionItems));
    }

    [Fact]
    public void Validate_Should_Fail_When_InspectionItems_Is_Empty()
    {
        // Arrange
        var command = CreateValidCommand(inspectionItems: []);

        // Act
        var result = _validator.Validate(command);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(CreateInspectionCommand.InspectionItems));
    }

    [Fact]
    public void Validate_Should_Fail_When_InspectionItems_Contains_Invalid_Item()
    {
        // Arrange
        var invalidItems = new List<CreateInspectionPassFailItemCommand>
        {
            new(0, true, "Test comment") // Invalid InspectionFormItemID
        };
        var command = CreateValidCommand(inspectionItems: invalidItems);

        // Act
        var result = _validator.Validate(command);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "InspectionItems[0].InspectionFormItemID");
    }

    [Fact]
    public void Validate_Should_Return_Multiple_Errors_For_Multiple_Invalid_Fields()
    {
        // Arrange
        var command = CreateValidCommand(
            inspectionFormId: 0,
            vehicleId: -1,
            technicianId: "",
            inspectionStartTime: DateTime.UtcNow.AddHours(1),
            inspectionEndTime: DateTime.UtcNow.AddHours(2),
            odometerReading: -50.0,
            notes: new string('a', NotesMaxLength + 1),
            inspectionItems: []);

        // Act
        var result = _validator.Validate(command);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(CreateInspectionCommand.InspectionFormID));
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(CreateInspectionCommand.VehicleID));
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(CreateInspectionCommand.TechnicianID));
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(CreateInspectionCommand.InspectionStartTime));
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(CreateInspectionCommand.InspectionEndTime));
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(CreateInspectionCommand.OdometerReading));
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(CreateInspectionCommand.Notes));
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(CreateInspectionCommand.InspectionItems));
    }
}