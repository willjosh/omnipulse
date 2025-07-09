using System;

using Domain.Entities.Enums;

using FluentValidation;

using Xunit;

namespace Application.Test.ServiceSchedules.CommandTest;

public abstract class ServiceScheduleCommandValidatorTestBase<TCommand, TValidator>
    where TValidator : IValidator<TCommand>
{
    protected abstract TValidator Validator { get; }

    protected abstract TCommand CreateValidCommand(
        int serviceProgramId = 1,
        string name = "5000 km / 6 week service",
        int? timeIntervalValue = 6,
        TimeUnitEnum? timeIntervalUnit = TimeUnitEnum.Weeks,
        int? timeBufferValue = 1,
        TimeUnitEnum? timeBufferUnit = TimeUnitEnum.Days,
        int? mileageInterval = 5000,
        int? mileageBuffer = 250,
        int? firstServiceTimeValue = null,
        TimeUnitEnum? firstServiceTimeUnit = null,
        int? firstServiceMileage = null,
        bool isActive = true);

    [Fact]
    public void Validator_Should_Not_Be_Null()
    {
        Assert.NotNull(Validator);
    }

    [Fact]
    public async Task Validator_Should_Pass_With_Valid_Command()
    {
        // Arrange
        var command = CreateValidCommand();

        // Act
        var result = await Validator.ValidateAsync(command);

        // Assert
        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }

    // SERVICE PROGRAM ID VALIDATION TESTS
    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-100)]
    public async Task Validator_Should_Fail_When_ServiceProgramID_Is_Invalid(int invalidServiceProgramID)
    {
        // Arrange
        var command = CreateValidCommand(serviceProgramId: invalidServiceProgramID);

        // Act
        var result = await Validator.ValidateAsync(command);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "ServiceProgramID");
    }

    // NAME VALIDATION TESTS
    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("\t")]
    [InlineData("\n")]
    public async Task Validator_Should_Fail_When_Name_Is_Empty_Or_Whitespace(string invalidName)
    {
        // Arrange
        var command = CreateValidCommand(name: invalidName);

        // Act
        var result = await Validator.ValidateAsync(command);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Name");
    }

    [Theory]
    [InlineData(201)]  // Exceeds 200 limit
    [InlineData(300)]  // Way over limit
    public async Task Validator_Should_Fail_When_Name_Exceeds_MaxLength(int nameLength)
    {
        // Arrange
        var command = CreateValidCommand(name: new string('A', nameLength));

        // Act
        var result = await Validator.ValidateAsync(command);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Name");
    }

    [Fact]
    public async Task Validator_Should_Pass_When_Name_At_MaxLength()
    {
        // Arrange
        var command = CreateValidCommand(name: new string('A', 200)); // Exactly 200 characters

        // Act
        var result = await Validator.ValidateAsync(command);

        // Assert
        Assert.True(result.IsValid);
    }

    // TIME INTERVAL VALIDATION TESTS
    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-100)]
    public async Task Validator_Should_Fail_When_TimeIntervalValue_Is_NonPositive(int invalidValue)
    {
        // Arrange
        var command = CreateValidCommand(timeIntervalValue: invalidValue);

        // Act
        var result = await Validator.ValidateAsync(command);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "TimeIntervalValue");
    }

    [Fact]
    public async Task Validator_Should_Fail_When_TimeIntervalValue_Without_Unit()
    {
        // Arrange
        var command = CreateValidCommand(timeIntervalValue: 6, timeIntervalUnit: null);

        // Act
        var result = await Validator.ValidateAsync(command);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "");
    }

    [Fact]
    public async Task Validator_Should_Fail_When_TimeIntervalUnit_Without_Value()
    {
        // Arrange
        var command = CreateValidCommand(timeIntervalValue: null, timeIntervalUnit: TimeUnitEnum.Days);

        // Act
        var result = await Validator.ValidateAsync(command);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "");
    }

    [Theory]
    [InlineData(TimeUnitEnum.Hours)]
    [InlineData(TimeUnitEnum.Days)]
    [InlineData(TimeUnitEnum.Weeks)]
    public async Task Validator_Should_Pass_When_TimeIntervalUnit_Is_Valid_Enum(TimeUnitEnum validUnit)
    {
        // Arrange
        var command = CreateValidCommand(
            timeIntervalValue: 1,
            timeIntervalUnit: validUnit,
            timeBufferValue: null,
            timeBufferUnit: null);

        // Act
        var result = await Validator.ValidateAsync(command);

        // Assert
        Assert.True(result.IsValid);
    }

    // MILEAGE INTERVAL VALIDATION TESTS
    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-1000)]
    public async Task Validator_Should_Fail_When_MileageInterval_Is_NonPositive(int invalidValue)
    {
        // Arrange
        var command = CreateValidCommand(
            timeIntervalValue: null,
            timeIntervalUnit: null,
            timeBufferValue: null,
            timeBufferUnit: null,
            mileageInterval: invalidValue);

        // Act
        var result = await Validator.ValidateAsync(command);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "MileageInterval");
    }

    // AT LEAST ONE INTERVAL REQUIRED TESTS
    [Fact]
    public async Task Validator_Should_Fail_When_No_Intervals_Provided()
    {
        // Arrange
        var command = CreateValidCommand(
            timeIntervalValue: null,
            timeIntervalUnit: null,
            timeBufferValue: null,
            timeBufferUnit: null,
            mileageInterval: null,
            mileageBuffer: null);

        // Act
        var result = await Validator.ValidateAsync(command);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "");
    }

    [Fact]
    public async Task Validator_Should_Pass_With_Only_TimeInterval()
    {
        // Arrange
        var command = CreateValidCommand(
            timeIntervalValue: 6,
            timeIntervalUnit: TimeUnitEnum.Days,
            mileageInterval: null,
            mileageBuffer: null);

        // Act
        var result = await Validator.ValidateAsync(command);

        // Assert
        Assert.True(result.IsValid);
    }

    [Fact]
    public async Task Validator_Should_Pass_With_Only_MileageInterval()
    {
        // Arrange
        var command = CreateValidCommand(
            timeIntervalValue: null,
            timeIntervalUnit: null,
            timeBufferValue: null,
            timeBufferUnit: null,
            mileageInterval: 5000);

        // Act
        var result = await Validator.ValidateAsync(command);

        // Assert
        Assert.True(result.IsValid);
    }

    // TIME BUFFER VALIDATION TESTS
    [Theory]
    [InlineData(-1)]
    [InlineData(-100)]
    public async Task Validator_Should_Fail_When_TimeBufferValue_Is_Negative(int invalidValue)
    {
        // Arrange
        var command = CreateValidCommand(
            timeBufferValue: invalidValue,
            timeBufferUnit: TimeUnitEnum.Days);

        // Act
        var result = await Validator.ValidateAsync(command);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "TimeBufferValue");
    }

    [Fact]
    public async Task Validator_Should_Pass_When_TimeBufferValue_Is_Zero()
    {
        // Arrange
        var command = CreateValidCommand(
            timeBufferValue: 0,
            timeBufferUnit: TimeUnitEnum.Days);

        // Act
        var result = await Validator.ValidateAsync(command);

        // Assert
        Assert.True(result.IsValid);
    }

    [Fact]
    public async Task Validator_Should_Fail_When_TimeBufferValue_Without_Unit()
    {
        // Arrange
        var command = CreateValidCommand(
            timeBufferValue: 1,
            timeBufferUnit: null);

        // Act
        var result = await Validator.ValidateAsync(command);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "");
    }

    [Fact]
    public async Task Validator_Should_Fail_When_TimeBufferUnit_Without_Value()
    {
        // Arrange
        var command = CreateValidCommand(
            timeBufferValue: null,
            timeBufferUnit: TimeUnitEnum.Days);

        // Act
        var result = await Validator.ValidateAsync(command);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "");
    }

    [Fact]
    public async Task Validator_Should_Fail_When_TimeBuffer_Equals_TimeInterval()
    {
        // Arrange
        var command = CreateValidCommand(
            timeIntervalValue: 7,
            timeIntervalUnit: TimeUnitEnum.Days,
            timeBufferValue: 7,
            timeBufferUnit: TimeUnitEnum.Days);

        // Act
        var result = await Validator.ValidateAsync(command);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "");
    }

    [Fact]
    public async Task Validator_Should_Fail_When_TimeBuffer_Greater_Than_TimeInterval()
    {
        // Arrange
        var command = CreateValidCommand(
            timeIntervalValue: 5,
            timeIntervalUnit: TimeUnitEnum.Days,
            timeBufferValue: 6,
            timeBufferUnit: TimeUnitEnum.Days);

        // Act
        var result = await Validator.ValidateAsync(command);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "");
    }

    [Fact]
    public async Task Validator_Should_Pass_When_TimeBuffer_Less_Than_TimeInterval()
    {
        // Arrange
        var command = CreateValidCommand(
            timeIntervalValue: 5,
            timeIntervalUnit: TimeUnitEnum.Days,
            timeBufferValue: 4,
            timeBufferUnit: TimeUnitEnum.Days);

        // Act
        var result = await Validator.ValidateAsync(command);

        // Assert
        Assert.True(result.IsValid);
    }

    // MILEAGE BUFFER VALIDATION TESTS
    [Theory]
    [InlineData(-1)]
    [InlineData(-1000)]
    public async Task Validator_Should_Fail_When_MileageBuffer_Is_Negative(int invalidValue)
    {
        // Arrange
        var command = CreateValidCommand(mileageBuffer: invalidValue);

        // Act
        var result = await Validator.ValidateAsync(command);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "MileageBuffer");
    }

    [Fact]
    public async Task Validator_Should_Pass_When_MileageBuffer_Is_Zero()
    {
        // Arrange
        var command = CreateValidCommand(mileageBuffer: 0);

        // Act
        var result = await Validator.ValidateAsync(command);

        // Assert
        Assert.True(result.IsValid);
    }

    [Fact]
    public async Task Validator_Should_Fail_When_MileageBuffer_Equals_MileageInterval()
    {
        // Arrange
        var command = CreateValidCommand(
            timeIntervalValue: null,
            timeIntervalUnit: null,
            mileageInterval: 5000,
            mileageBuffer: 5000);

        // Act
        var result = await Validator.ValidateAsync(command);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "");
    }

    [Fact]
    public async Task Validator_Should_Fail_When_MileageBuffer_Greater_Than_MileageInterval()
    {
        // Arrange
        var command = CreateValidCommand(
            mileageInterval: 1000,
            mileageBuffer: 1001);

        // Act
        var result = await Validator.ValidateAsync(command);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "");
    }

    [Fact]
    public async Task Validator_Should_Pass_When_MileageBuffer_Less_Than_MileageInterval()
    {
        // Arrange
        var command = CreateValidCommand(
            mileageInterval: 1000,
            mileageBuffer: 999);

        // Act
        var result = await Validator.ValidateAsync(command);

        // Assert
        Assert.True(result.IsValid);
    }

    // FIRST SERVICE TIME VALIDATION TESTS
    [Theory]
    [InlineData(-1)]
    [InlineData(-100)]
    public async Task Validator_Should_Fail_When_FirstServiceTimeValue_Is_Negative(int invalidValue)
    {
        // Arrange
        var command = CreateValidCommand(firstServiceTimeValue: invalidValue, firstServiceTimeUnit: TimeUnitEnum.Days);

        // Act
        var result = await Validator.ValidateAsync(command);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "FirstServiceTimeValue");
    }

    [Fact]
    public async Task Validator_Should_Pass_When_FirstServiceTimeValue_Is_Zero()
    {
        // Arrange
        var command = CreateValidCommand(firstServiceTimeValue: 0, firstServiceTimeUnit: TimeUnitEnum.Days);

        // Act
        var result = await Validator.ValidateAsync(command);

        // Assert
        Assert.True(result.IsValid);
    }

    [Fact]
    public async Task Validator_Should_Fail_When_FirstServiceTimeValue_Without_Unit()
    {
        // Arrange
        var command = CreateValidCommand(firstServiceTimeValue: 1, firstServiceTimeUnit: null);

        // Act
        var result = await Validator.ValidateAsync(command);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "");
    }

    [Fact]
    public async Task Validator_Should_Fail_When_FirstServiceTimeUnit_Without_Value()
    {
        // Arrange
        var command = CreateValidCommand(firstServiceTimeValue: null, firstServiceTimeUnit: TimeUnitEnum.Days);

        // Act
        var result = await Validator.ValidateAsync(command);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "");
    }

    [Fact]
    public async Task Validator_Should_Fail_When_FirstServiceTime_Without_TimeInterval()
    {
        // Arrange
        var command = CreateValidCommand(
            timeIntervalValue: null,
            timeIntervalUnit: null,
            timeBufferValue: null,
            timeBufferUnit: null,
            firstServiceTimeValue: 2,
            firstServiceTimeUnit: TimeUnitEnum.Days);

        // Act
        var result = await Validator.ValidateAsync(command);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "");
    }

    // FIRST SERVICE MILEAGE VALIDATION TESTS
    [Theory]
    [InlineData(-1)]
    [InlineData(-1000)]
    public async Task Validator_Should_Fail_When_FirstServiceMileage_Is_Negative(int invalidValue)
    {
        // Arrange
        var command = CreateValidCommand(firstServiceMileage: invalidValue);

        // Act
        var result = await Validator.ValidateAsync(command);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "FirstServiceMileage");
    }

    [Fact]
    public async Task Validator_Should_Pass_When_FirstServiceMileage_Is_Zero()
    {
        // Arrange
        var command = CreateValidCommand(firstServiceMileage: 0);

        // Act
        var result = await Validator.ValidateAsync(command);

        // Assert
        Assert.True(result.IsValid);
    }

    [Fact]
    public async Task Validator_Should_Fail_When_FirstServiceMileage_Without_MileageInterval()
    {
        // Arrange
        var command = CreateValidCommand(
            timeIntervalValue: null,
            timeIntervalUnit: null,
            timeBufferValue: null,
            timeBufferUnit: null,
            mileageInterval: null,
            mileageBuffer: null,
            firstServiceMileage: 10000);

        // Act
        var result = await Validator.ValidateAsync(command);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "");
    }

    [Fact]
    [Trait("Category", "Positive")]
    public async Task Validator_Should_Pass_With_All_Fields_Provided()
    {
        // Arrange
        var command = CreateValidCommand(
            timeIntervalValue: 6,
            timeIntervalUnit: TimeUnitEnum.Days,
            timeBufferValue: 1,
            timeBufferUnit: TimeUnitEnum.Days,
            mileageInterval: 5000,
            mileageBuffer: 250,
            firstServiceTimeValue: 3,
            firstServiceTimeUnit: TimeUnitEnum.Days,
            firstServiceMileage: 2500);

        // Act
        var result = await Validator.ValidateAsync(command);

        // Assert
        Assert.True(result.IsValid);
    }

    [Fact]
    [Trait("Category", "TimeUnit")]
    public async Task Validator_Should_Fail_When_TimeBuffer_Different_Units_Greater()
    {
        // Arrange - 4 weeks = 28 days, which is greater than 5 days
        var command = CreateValidCommand(
            timeIntervalValue: 5,
            timeIntervalUnit: TimeUnitEnum.Days,
            timeBufferValue: 4,
            timeBufferUnit: TimeUnitEnum.Weeks,
            mileageInterval: null,
            mileageBuffer: null);

        // Act
        var result = await Validator.ValidateAsync(command);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "");
    }

    [Fact]
    [Trait("Category", "TimeUnit")]
    public async Task Validator_Should_Pass_When_TimeBuffer_Different_Units_Smaller()
    {
        // Arrange - 4 days vs 1 week (4 < 7)
        var command = CreateValidCommand(
            timeIntervalValue: 1,
            timeIntervalUnit: TimeUnitEnum.Weeks,
            timeBufferValue: 4,
            timeBufferUnit: TimeUnitEnum.Days,
            mileageInterval: null,
            mileageBuffer: null);

        // Act
        var result = await Validator.ValidateAsync(command);

        // Assert
        Assert.True(result.IsValid);
    }

    [Fact]
    [Trait("Category", "Robustness")]
    public async Task Validator_Should_Pass_With_All_Optional_Fields_Provided_Time_Based()
    {
        // Arrange
        var command = CreateValidCommand(
            timeIntervalValue: 12,
            timeIntervalUnit: TimeUnitEnum.Days,
            timeBufferValue: 2,
            timeBufferUnit: TimeUnitEnum.Days,
            mileageInterval: null,
            mileageBuffer: null,
            firstServiceTimeValue: 6,
            firstServiceTimeUnit: TimeUnitEnum.Days,
            firstServiceMileage: null);

        // Act
        var result = await Validator.ValidateAsync(command);

        // Assert
        Assert.True(result.IsValid);
    }

    [Fact]
    [Trait("Category", "Robustness")]
    public async Task Validator_Should_Pass_With_All_Optional_Fields_Provided_Mileage_Based()
    {
        // Arrange
        var command = CreateValidCommand(
            timeIntervalValue: null,
            timeIntervalUnit: null,
            timeBufferValue: null,
            timeBufferUnit: null,
            mileageInterval: 10000,
            mileageBuffer: 500,
            firstServiceTimeValue: null,
            firstServiceTimeUnit: null,
            firstServiceMileage: 5000);

        // Act
        var result = await Validator.ValidateAsync(command);

        // Assert
        Assert.True(result.IsValid);
    }

    [Fact]
    [Trait("Category", "Robustness")]
    public async Task Validator_Should_Pass_With_Maximum_Valid_Configuration()
    {
        // Arrange
        var command = CreateValidCommand(
            timeIntervalValue: 12,
            timeIntervalUnit: TimeUnitEnum.Days,
            timeBufferValue: 11,
            timeBufferUnit: TimeUnitEnum.Days,
            mileageInterval: 10000,
            mileageBuffer: 9999,
            firstServiceTimeValue: 6,
            firstServiceTimeUnit: TimeUnitEnum.Days,
            firstServiceMileage: 5000);

        // Act
        var result = await Validator.ValidateAsync(command);

        // Assert
        Assert.True(result.IsValid);
    }

    [Fact]
    [Trait("Category", "EdgeCase")]
    public async Task Validator_Should_Pass_When_TimeBuffer_One_Less_Than_TimeInterval()
    {
        // Arrange
        var command = CreateValidCommand(
            timeIntervalValue: 7,
            timeIntervalUnit: TimeUnitEnum.Days,
            timeBufferValue: 6,
            timeBufferUnit: TimeUnitEnum.Days,
            mileageInterval: null,
            mileageBuffer: null);

        // Act
        var result = await Validator.ValidateAsync(command);

        // Assert
        Assert.True(result.IsValid);
    }

    [Fact]
    [Trait("Category", "EdgeCase")]
    public async Task Validator_Should_Pass_When_MileageBuffer_One_Less_Than_MileageInterval()
    {
        // Arrange
        var command = CreateValidCommand(
            timeIntervalValue: null,
            timeIntervalUnit: null,
            mileageInterval: 5000,
            mileageBuffer: 4999);

        // Act
        var result = await Validator.ValidateAsync(command);

        // Assert
        Assert.True(result.IsValid);
    }

    [Fact]
    [Trait("Category", "Unicode")]
    public async Task Validator_Should_Pass_With_Unicode_Name()
    {
        // Arrange
        var command = CreateValidCommand(
            name: "˜ˆç˙ø¬åß Âå††˙´∑ Íå∑",
            timeBufferValue: 1,
            timeBufferUnit: TimeUnitEnum.Days,
            mileageBuffer: 250);

        // Act
        var result = await Validator.ValidateAsync(command);

        // Assert
        Assert.True(result.IsValid);
    }

    [Fact]
    [Trait("Category", "Boundary")]
    public async Task Validator_Should_Pass_With_Shortest_Valid_Name()
    {
        // Arrange
        var command = CreateValidCommand(
            name: "A",
            timeBufferValue: 1,
            timeBufferUnit: TimeUnitEnum.Days,
            mileageBuffer: 250);

        // Act
        var result = await Validator.ValidateAsync(command);

        // Assert
        Assert.True(result.IsValid);
    }

    [Fact]
    [Trait("Category", "EnumValidation")]
    public async Task Validator_Should_Fail_When_TimeIntervalUnit_InvalidEnum()
    {
        // Arrange
        var command = CreateValidCommand(
            timeIntervalValue: 5,
            timeIntervalUnit: (TimeUnitEnum)999, // invalid enum
            mileageInterval: null,
            mileageBuffer: null);

        // Act
        var result = await Validator.ValidateAsync(command);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "TimeIntervalUnit");
    }

    [Fact]
    [Trait("Category", "EnumValidation")]
    public async Task Validator_Should_Fail_When_TimeBufferUnit_InvalidEnum()
    {
        // Arrange
        var command = CreateValidCommand(
            timeIntervalValue: 5,
            timeIntervalUnit: TimeUnitEnum.Days,
            timeBufferValue: 1,
            timeBufferUnit: (TimeUnitEnum)999, // invalid enum
            mileageInterval: null,
            mileageBuffer: null);

        // Act
        var result = await Validator.ValidateAsync(command);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "TimeBufferUnit");
    }

    [Fact]
    [Trait("Category", "EnumValidation")]
    public async Task Validator_Should_Fail_When_FirstServiceTimeUnit_InvalidEnum()
    {
        // Arrange
        var command = CreateValidCommand(
            timeIntervalValue: 5,
            timeIntervalUnit: TimeUnitEnum.Days,
            firstServiceTimeValue: 1,
            firstServiceTimeUnit: (TimeUnitEnum)999);

        // Act
        var result = await Validator.ValidateAsync(command);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "FirstServiceTimeUnit");
    }

    [Fact]
    [Trait("Category", "FirstServiceEdge")]
    public async Task Validator_Should_Fail_When_Both_FirstService_Without_Any_Intervals()
    {
        // Arrange
        var command = CreateValidCommand(
            timeIntervalValue: null,
            timeIntervalUnit: null,
            timeBufferValue: null,
            timeBufferUnit: null,
            mileageInterval: null,
            mileageBuffer: null,
            firstServiceTimeValue: 3,
            firstServiceTimeUnit: TimeUnitEnum.Days,
            firstServiceMileage: 1000);

        // Act
        var result = await Validator.ValidateAsync(command);

        // Assert
        Assert.False(result.IsValid);
        // Should fail for multiple reasons: no intervals provided AND FirstService requires intervals
        Assert.Contains(result.Errors, e => e.PropertyName == "");
    }

    [Fact]
    [Trait("Category", "FirstServiceEdge")]
    public async Task Validator_Should_Fail_When_FirstServiceTime_Without_TimeInterval_But_With_MileageInterval()
    {
        // Arrange
        var command = CreateValidCommand(
            timeIntervalValue: null,
            timeIntervalUnit: null,
            timeBufferValue: null,
            timeBufferUnit: null,
            mileageInterval: 5000,
            mileageBuffer: null,
            firstServiceTimeValue: 3,
            firstServiceTimeUnit: TimeUnitEnum.Days,
            firstServiceMileage: null);

        // Act
        var result = await Validator.ValidateAsync(command);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "");
    }

    [Fact]
    [Trait("Category", "FirstServiceEdge")]
    public async Task Validator_Should_Fail_When_FirstServiceMileage_Without_MileageInterval_But_With_TimeInterval()
    {
        // Arrange
        var command = CreateValidCommand(
            timeIntervalValue: 6,
            timeIntervalUnit: TimeUnitEnum.Days,
            timeBufferValue: null,
            timeBufferUnit: null,
            mileageInterval: null,
            mileageBuffer: null,
            firstServiceTimeValue: null,
            firstServiceTimeUnit: null,
            firstServiceMileage: 1000);

        // Act
        var result = await Validator.ValidateAsync(command);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "");
    }

    [Fact]
    [Trait("Category", "FirstServiceEdge")]
    public async Task Validator_Should_Pass_When_FirstService_Properties_Match_Available_Intervals()
    {
        // Arrange
        var command = CreateValidCommand(
            timeIntervalValue: 6,
            timeIntervalUnit: TimeUnitEnum.Days,
            timeBufferValue: null,
            timeBufferUnit: null,
            mileageInterval: 5000,
            mileageBuffer: null,
            firstServiceTimeValue: 3,
            firstServiceTimeUnit: TimeUnitEnum.Days,
            firstServiceMileage: 2500);

        // Act
        var result = await Validator.ValidateAsync(command);

        // Assert
        Assert.True(result.IsValid);
    }

    [Fact]
    [Trait("Category", "FirstServiceEdge")]
    public async Task Validator_Should_Pass_With_FirstService_Equal_To_Interval()
    {
        // Arrange
        var command = CreateValidCommand(
            timeIntervalValue: 6,
            timeIntervalUnit: TimeUnitEnum.Days,
            timeBufferValue: null,
            timeBufferUnit: null,
            firstServiceTimeValue: 6,
            firstServiceTimeUnit: TimeUnitEnum.Days,
            mileageInterval: 5000,
            firstServiceMileage: 5000);

        // Act
        var result = await Validator.ValidateAsync(command);

        // Assert
        Assert.True(result.IsValid);
    }

    [Fact]
    [Trait("Category", "FirstServiceEdge")]
    public async Task Validator_Should_Pass_With_FirstService_Greater_Than_Interval()
    {
        // Arrange
        var command = CreateValidCommand(
            timeIntervalValue: 6,
            timeIntervalUnit: TimeUnitEnum.Days,
            timeBufferValue: null,
            timeBufferUnit: null,
            firstServiceTimeValue: 12,
            firstServiceTimeUnit: TimeUnitEnum.Days,
            mileageInterval: 5000,
            firstServiceMileage: 10000);

        // Act
        var result = await Validator.ValidateAsync(command);

        // Assert
        Assert.True(result.IsValid);
    }

    [Fact]
    [Trait("Category", "PositiveScenario")]
    public async Task Validator_Should_Pass_With_Hours_TimeUnit()
    {
        // Arrange
        var command = CreateValidCommand(
            timeIntervalValue: 48,
            timeIntervalUnit: TimeUnitEnum.Hours,
            timeBufferValue: 1,
            timeBufferUnit: TimeUnitEnum.Hours,
            mileageInterval: null,
            mileageBuffer: null);

        // Act
        var result = await Validator.ValidateAsync(command);

        // Assert
        Assert.True(result.IsValid);
    }

    [Fact]
    [Trait("Category", "PositiveScenario")]
    public async Task Validator_Should_Pass_With_Large_Interval_And_Buffer()
    {
        // Arrange
        var command = CreateValidCommand(
            timeIntervalValue: 24,
            timeIntervalUnit: TimeUnitEnum.Days,
            timeBufferValue: 23,
            timeBufferUnit: TimeUnitEnum.Days,
            mileageInterval: null,
            mileageBuffer: null);

        // Act
        var result = await Validator.ValidateAsync(command);

        // Assert
        Assert.True(result.IsValid);
    }

    [Fact]
    [Trait("Category", "EdgeCase")]
    public async Task Validator_Should_Fail_When_Buffer_Equals_Interval_Different_Units()
    {
        // Arrange
        var command = CreateValidCommand(
            timeIntervalValue: 4,
            timeIntervalUnit: TimeUnitEnum.Weeks,
            timeBufferValue: 28,
            timeBufferUnit: TimeUnitEnum.Days,
            mileageInterval: null,
            mileageBuffer: null);

        // Act
        var result = await Validator.ValidateAsync(command);

        // Assert
        Assert.False(result.IsValid);
    }

    [Fact]
    [Trait("Category", "EdgeCase")]
    public async Task Validator_Should_Fail_When_Weeks_Buffer_Greater_Than_Days_Interval()
    {
        // Arrange
        var command = CreateValidCommand(
            timeIntervalValue: 10,
            timeIntervalUnit: TimeUnitEnum.Days,
            timeBufferValue: 2,
            timeBufferUnit: TimeUnitEnum.Weeks,
            mileageInterval: null,
            mileageBuffer: null);

        // Act
        var result = await Validator.ValidateAsync(command);

        // Assert
        Assert.False(result.IsValid);
    }

    [Fact]
    [Trait("Category", "General")]
    public async Task Validator_Should_Pass_When_IsActive_Is_False()
    {
        // Arrange
        var command = CreateValidCommand(isActive: false);

        // Act
        var result = await Validator.ValidateAsync(command);

        // Assert
        Assert.True(result.IsValid);
    }
}