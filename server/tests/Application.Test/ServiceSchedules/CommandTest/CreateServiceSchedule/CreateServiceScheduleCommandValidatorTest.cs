using System;

using Application.Features.ServiceSchedules.Command.CreateServiceSchedule;

using Domain.Entities.Enums;

using Xunit;

namespace Application.Test.ServiceSchedules.CommandTest.CreateServiceSchedule;

public class CreateServiceScheduleCommandValidatorTest
{
    private readonly CreateServiceScheduleCommandValidator _validator;

    public CreateServiceScheduleCommandValidatorTest()
    {
        _validator = new CreateServiceScheduleCommandValidator();
    }

    private static CreateServiceScheduleCommand CreateValidCommand(
        int serviceProgramId = 1,
        string name = "5000 km / 6-day service",
        int? timeIntervalValue = 6,
        TimeUnitEnum? timeIntervalUnit = TimeUnitEnum.Days,
        int? timeBufferValue = null,
        TimeUnitEnum? timeBufferUnit = null,
        int? mileageInterval = 5000,
        int? mileageBuffer = null,
        int? firstServiceTimeValue = null,
        TimeUnitEnum? firstServiceTimeUnit = null,
        int? firstServiceMileage = null,
        bool isActive = true) => new(
            ServiceProgramID: serviceProgramId,
            Name: name,
            TimeIntervalValue: timeIntervalValue,
            TimeIntervalUnit: timeIntervalUnit,
            TimeBufferValue: timeBufferValue,
            TimeBufferUnit: timeBufferUnit,
            MileageInterval: mileageInterval,
            MileageBuffer: mileageBuffer,
            FirstServiceTimeValue: firstServiceTimeValue,
            FirstServiceTimeUnit: firstServiceTimeUnit,
            FirstServiceMileage: firstServiceMileage,
            IsActive: isActive);

    [Fact]
    public async Task Validator_Should_Pass_With_Valid_Command()
    {
        // Arrange
        var command = CreateValidCommand();

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }

    #region ServiceProgramID Tests

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-100)]
    public async Task Should_Fail_When_ServiceProgramID_Invalid(int invalidId)
    {
        var command = CreateValidCommand(serviceProgramId: invalidId);
        var result = await _validator.ValidateAsync(command);
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "ServiceProgramID");
    }

    #endregion

    #region Name Tests

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("\t")]
    [InlineData("\n")]
    public async Task Should_Fail_When_Name_Empty_Or_Whitespace(string invalidName)
    {
        var command = CreateValidCommand(name: invalidName);
        var result = await _validator.ValidateAsync(command);
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Name");
    }

    [Fact]
    public async Task Should_Fail_When_Name_Exceeds_MaxLength()
    {
        var longName = new string('a', 201); // Exceeds 200 character limit
        var command = CreateValidCommand(name: longName);
        var result = await _validator.ValidateAsync(command);
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Name");
    }

    [Fact]
    public async Task Should_Pass_When_Name_At_MaxLength()
    {
        var maxLengthName = new string('a', 200); // Exactly 200 characters
        var command = CreateValidCommand(name: maxLengthName);
        var result = await _validator.ValidateAsync(command);
        Assert.True(result.IsValid);
    }

    #endregion

    #region TimeInterval Tests

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-100)]
    public async Task Should_Fail_When_TimeIntervalValue_NonPositive(int invalidValue)
    {
        var command = CreateValidCommand(timeIntervalValue: invalidValue);
        var result = await _validator.ValidateAsync(command);
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "TimeIntervalValue");
    }

    [Fact]
    public async Task Should_Fail_When_TimeIntervalValue_Without_Unit()
    {
        var command = CreateValidCommand(timeIntervalValue: 6, timeIntervalUnit: null);
        var result = await _validator.ValidateAsync(command);
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "");
    }

    [Fact]
    public async Task Should_Fail_When_TimeIntervalUnit_Without_Value()
    {
        var command = CreateValidCommand(timeIntervalValue: null, timeIntervalUnit: TimeUnitEnum.Days);
        var result = await _validator.ValidateAsync(command);
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "");
    }

    [Theory]
    [InlineData(TimeUnitEnum.Hours)]
    [InlineData(TimeUnitEnum.Days)]
    [InlineData(TimeUnitEnum.Weeks)]
    public async Task Should_Pass_When_TimeIntervalUnit_Valid_Enum(TimeUnitEnum validUnit)
    {
        var command = CreateValidCommand(
            timeIntervalValue: 1,
            timeIntervalUnit: validUnit,
            mileageInterval: null,
            mileageBuffer: null);
        var result = await _validator.ValidateAsync(command);
        Assert.True(result.IsValid);
    }

    #endregion

    #region MileageInterval Tests

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-1000)]
    public async Task Should_Fail_When_MileageInterval_NonPositive(int invalidValue)
    {
        var command = CreateValidCommand(
            timeIntervalValue: null,
            timeIntervalUnit: null,
            timeBufferValue: null,
            timeBufferUnit: null,
            mileageInterval: invalidValue);
        var result = await _validator.ValidateAsync(command);
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "MileageInterval");
    }

    #endregion

    #region At Least One Interval Required Tests

    [Fact]
    public async Task Should_Fail_When_No_Intervals_Provided()
    {
        var command = CreateValidCommand(
            timeIntervalValue: null,
            timeIntervalUnit: null,
            timeBufferValue: null,
            timeBufferUnit: null,
            mileageInterval: null,
            mileageBuffer: null);
        var result = await _validator.ValidateAsync(command);
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "");
    }

    #endregion

    #region TimeBuffer Tests

    [Theory]
    [InlineData(-1)]
    [InlineData(-100)]
    public async Task Should_Fail_When_TimeBufferValue_Negative(int invalidValue)
    {
        var command = CreateValidCommand(
            timeBufferValue: invalidValue,
            timeBufferUnit: TimeUnitEnum.Days);
        var result = await _validator.ValidateAsync(command);
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "TimeBufferValue");
    }

    [Fact]
    public async Task Should_Pass_When_TimeBufferValue_Zero()
    {
        var command = CreateValidCommand(
            timeBufferValue: 0,
            timeBufferUnit: TimeUnitEnum.Days);
        var result = await _validator.ValidateAsync(command);
        Assert.True(result.IsValid);
    }

    [Fact]
    public async Task Should_Fail_When_TimeBufferValue_Without_Unit()
    {
        var command = CreateValidCommand(
            timeBufferValue: 1,
            timeBufferUnit: null);
        var result = await _validator.ValidateAsync(command);
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "");
    }

    [Fact]
    public async Task Should_Fail_When_TimeBufferUnit_Without_Value()
    {
        var command = CreateValidCommand(
            timeBufferValue: null,
            timeBufferUnit: TimeUnitEnum.Days);
        var result = await _validator.ValidateAsync(command);
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "");
    }

    [Fact]
    public async Task Should_Fail_When_TimeBuffer_Equals_TimeInterval()
    {
        var command = CreateValidCommand(
            timeIntervalValue: 5,
            timeIntervalUnit: TimeUnitEnum.Days,
            timeBufferValue: 5,
            timeBufferUnit: TimeUnitEnum.Days);
        var result = await _validator.ValidateAsync(command);
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "");
    }

    [Fact]
    public async Task Should_Fail_When_TimeBuffer_Greater_Than_TimeInterval()
    {
        var command = CreateValidCommand(
            timeIntervalValue: 5,
            timeIntervalUnit: TimeUnitEnum.Days,
            timeBufferValue: 6,
            timeBufferUnit: TimeUnitEnum.Days);
        var result = await _validator.ValidateAsync(command);
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "");
    }

    [Fact]
    public async Task Should_Pass_When_TimeBuffer_Less_Than_TimeInterval()
    {
        var command = CreateValidCommand(
            timeIntervalValue: 5,
            timeIntervalUnit: TimeUnitEnum.Days,
            timeBufferValue: 4,
            timeBufferUnit: TimeUnitEnum.Days);
        var result = await _validator.ValidateAsync(command);
        Assert.True(result.IsValid);
    }

    #endregion

    #region MileageBuffer Tests

    [Theory]
    [InlineData(-1)]
    [InlineData(-1000)]
    public async Task Should_Fail_When_MileageBuffer_Negative(int invalidValue)
    {
        var command = CreateValidCommand(
            mileageBuffer: invalidValue);
        var result = await _validator.ValidateAsync(command);
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "MileageBuffer");
    }

    [Fact]
    public async Task Should_Pass_When_MileageBuffer_Zero()
    {
        var command = CreateValidCommand(
            mileageBuffer: 0);
        var result = await _validator.ValidateAsync(command);
        Assert.True(result.IsValid);
    }

    [Fact]
    public async Task Should_Fail_When_MileageBuffer_Equals_MileageInterval()
    {
        var command = CreateValidCommand(
            mileageInterval: 1000,
            mileageBuffer: 1000);
        var result = await _validator.ValidateAsync(command);
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "");
    }

    [Fact]
    public async Task Should_Fail_When_MileageBuffer_Greater_Than_MileageInterval()
    {
        var command = CreateValidCommand(
            mileageInterval: 1000,
            mileageBuffer: 1001);
        var result = await _validator.ValidateAsync(command);
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "");
    }

    [Fact]
    public async Task Should_Pass_When_MileageBuffer_Less_Than_MileageInterval()
    {
        var command = CreateValidCommand(
            mileageInterval: 1000,
            mileageBuffer: 999);
        var result = await _validator.ValidateAsync(command);
        Assert.True(result.IsValid);
    }

    #endregion

    #region FirstServiceTime Tests

    [Theory]
    [InlineData(-1)]
    [InlineData(-100)]
    public async Task Should_Fail_When_FirstServiceTimeValue_Negative(int invalidValue)
    {
        var command = CreateValidCommand(firstServiceTimeValue: invalidValue, firstServiceTimeUnit: TimeUnitEnum.Days);
        var result = await _validator.ValidateAsync(command);
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "FirstServiceTimeValue");
    }

    [Fact]
    public async Task Should_Fail_When_FirstServiceTimeValue_Without_Unit()
    {
        var command = CreateValidCommand(firstServiceTimeValue: 1, firstServiceTimeUnit: null);
        var result = await _validator.ValidateAsync(command);
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "");
    }

    [Fact]
    public async Task Should_Fail_When_FirstServiceTimeUnit_Without_Value()
    {
        var command = CreateValidCommand(firstServiceTimeValue: null, firstServiceTimeUnit: TimeUnitEnum.Days);
        var result = await _validator.ValidateAsync(command);
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "");
    }

    [Fact]
    public async Task Should_Fail_When_FirstServiceTime_Without_TimeInterval()
    {
        var command = CreateValidCommand(
            timeIntervalValue: null,
            timeIntervalUnit: null,
            timeBufferValue: null,
            timeBufferUnit: null,
            firstServiceTimeValue: 2,
            firstServiceTimeUnit: TimeUnitEnum.Days);
        var result = await _validator.ValidateAsync(command);
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "");
    }

    #endregion

    #region FirstServiceMileage Tests

    [Theory]
    [InlineData(-1)]
    [InlineData(-1000)]
    public async Task Should_Fail_When_FirstServiceMileage_Negative(int invalidValue)
    {
        var command = CreateValidCommand(firstServiceMileage: invalidValue);
        var result = await _validator.ValidateAsync(command);
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "FirstServiceMileage");
    }

    [Fact]
    public async Task Should_Fail_When_FirstServiceMileage_Without_MileageInterval()
    {
        var command = CreateValidCommand(
            timeIntervalValue: null,
            timeIntervalUnit: null,
            timeBufferValue: null,
            timeBufferUnit: null,
            mileageInterval: null,
            mileageBuffer: null,
            firstServiceMileage: 10000);
        var result = await _validator.ValidateAsync(command);
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "");
    }

    #endregion

    [Fact]
    public async Task Should_Pass_With_MaxBuffer_Values()
    {
        var command = CreateValidCommand(
            timeIntervalValue: 10,
            timeIntervalUnit: TimeUnitEnum.Days,
            timeBufferValue: 9,
            timeBufferUnit: TimeUnitEnum.Days,
            mileageInterval: 1000,
            mileageBuffer: 999);
        var result = await _validator.ValidateAsync(command);
        Assert.True(result.IsValid);
    }

    [Fact]
    public async Task Should_Pass_With_Different_TimeUnits()
    {
        var command = CreateValidCommand(
            timeIntervalValue: 1,
            timeIntervalUnit: TimeUnitEnum.Weeks,
            timeBufferValue: 1,
            timeBufferUnit: TimeUnitEnum.Days,
            firstServiceTimeValue: 1,
            firstServiceTimeUnit: TimeUnitEnum.Hours,
            mileageInterval: null,
            mileageBuffer: null);
        var result = await _validator.ValidateAsync(command);
        Assert.True(result.IsValid);
    }

    [Fact]
    public async Task Should_Pass_With_FirstService_Equal_To_Interval()
    {
        var command = CreateValidCommand(
            timeIntervalValue: 6,
            timeIntervalUnit: TimeUnitEnum.Days,
            timeBufferValue: null,
            timeBufferUnit: null,
            firstServiceTimeValue: 6,
            firstServiceTimeUnit: TimeUnitEnum.Days,
            mileageInterval: 5000,
            firstServiceMileage: 5000);
        var result = await _validator.ValidateAsync(command);
        Assert.True(result.IsValid);
    }

    [Fact]
    public async Task Should_Pass_With_FirstService_Greater_Than_Interval()
    {
        var command = CreateValidCommand(
            timeIntervalValue: 6,
            timeIntervalUnit: TimeUnitEnum.Days,
            timeBufferValue: null,
            timeBufferUnit: null,
            firstServiceTimeValue: 12,
            firstServiceTimeUnit: TimeUnitEnum.Days,
            mileageInterval: 5000,
            firstServiceMileage: 10000);
        var result = await _validator.ValidateAsync(command);
        Assert.True(result.IsValid);
    }

    [Fact]
    public async Task Should_Pass_With_All_Fields_Provided()
    {
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
        var result = await _validator.ValidateAsync(command);
        Assert.True(result.IsValid);
    }

    #region Mismatched Units Tests

    [Fact]
    public async Task Should_Fail_When_TimeBuffer_Different_Units_Potentially_Greater()
    {
        // 4 weeks = 28 days, which is greater than 5 days
        var command = CreateValidCommand(
            timeIntervalValue: 5,
            timeIntervalUnit: TimeUnitEnum.Days,
            timeBufferValue: 4,
            timeBufferUnit: TimeUnitEnum.Weeks,
            mileageInterval: null,
            mileageBuffer: null);
        var result = await _validator.ValidateAsync(command);
        Assert.False(result.IsValid);
    }

    [Fact]
    public async Task Should_Pass_When_TimeBuffer_Different_Units_Potentially_Smaller()
    {
        // 4 days vs 1 week (4 < 7)
        var command = CreateValidCommand(
            timeIntervalValue: 1,
            timeIntervalUnit: TimeUnitEnum.Weeks,
            timeBufferValue: 4,
            timeBufferUnit: TimeUnitEnum.Days,
            mileageInterval: null,
            mileageBuffer: null);
        var result = await _validator.ValidateAsync(command);
        Assert.True(result.IsValid);
    }

    [Fact]
    public async Task Should_Fail_When_Buffer_Equals_Interval_Different_Units()
    {
        var command = CreateValidCommand(
            timeIntervalValue: 4,
            timeIntervalUnit: TimeUnitEnum.Weeks,
            timeBufferValue: 28,
            timeBufferUnit: TimeUnitEnum.Days,
            mileageInterval: null,
            mileageBuffer: null);
        var result = await _validator.ValidateAsync(command);
        Assert.False(result.IsValid);
    }

    [Fact]
    public async Task Should_Fail_When_Weeks_Buffer_Greater_Than_Days_Interval()
    {
        var command = CreateValidCommand(
            timeIntervalValue: 10,
            timeIntervalUnit: TimeUnitEnum.Days,
            timeBufferValue: 2,
            timeBufferUnit: TimeUnitEnum.Weeks,
            mileageInterval: null,
            mileageBuffer: null);
        var result = await _validator.ValidateAsync(command);
        Assert.False(result.IsValid);
    }

    #endregion

    #region Null and Zero Combinations

    [Fact]
    public async Task Should_Fail_When_Name_Is_Null()
    {
        var command = new CreateServiceScheduleCommand(
            ServiceProgramID: 1,
            Name: null!,
            TimeIntervalValue: 6,
            TimeIntervalUnit: TimeUnitEnum.Days,
            TimeBufferValue: null,
            TimeBufferUnit: null,
            MileageInterval: null,
            MileageBuffer: null,
            FirstServiceTimeValue: null,
            FirstServiceTimeUnit: null,
            FirstServiceMileage: null,
            IsActive: true);
        var result = await _validator.ValidateAsync(command);
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Name");
    }

    [Fact]
    public async Task Should_Fail_When_TimeIntervalValue_Is_Zero()
    {
        var command = CreateValidCommand(
            timeIntervalValue: 0,
            timeIntervalUnit: TimeUnitEnum.Days,
            mileageInterval: null,
            mileageBuffer: null);
        var result = await _validator.ValidateAsync(command);
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "TimeIntervalValue");
    }

    [Fact]
    public async Task Should_Pass_When_TimeBufferValue_Is_Zero_With_Unit()
    {
        var command = CreateValidCommand(
            timeIntervalValue: 5,
            timeIntervalUnit: TimeUnitEnum.Days,
            timeBufferValue: 0,
            timeBufferUnit: TimeUnitEnum.Days,
            mileageInterval: null,
            mileageBuffer: null);
        var result = await _validator.ValidateAsync(command);
        Assert.True(result.IsValid);
    }

    [Fact]
    public async Task Should_Fail_When_MileageInterval_Is_Zero()
    {
        var command = CreateValidCommand(
            timeIntervalValue: null,
            timeIntervalUnit: null,
            mileageInterval: 0,
            mileageBuffer: null);
        var result = await _validator.ValidateAsync(command);
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "MileageInterval");
    }

    [Fact]
    public async Task Should_Pass_When_MileageBuffer_Is_Zero()
    {
        var command = CreateValidCommand(
            mileageInterval: 1000,
            mileageBuffer: 0);
        var result = await _validator.ValidateAsync(command);
        Assert.True(result.IsValid);
    }

    #endregion

    #region Combined FirstService Scenarios

    [Fact]
    public async Task Should_Fail_When_Both_FirstService_Without_Any_Intervals()
    {
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
        var result = await _validator.ValidateAsync(command);
        Assert.False(result.IsValid);
        // Should fail for multiple reasons: no intervals provided AND FirstService requires intervals
        Assert.Contains(result.Errors, e => e.PropertyName == "");
    }

    [Fact]
    public async Task Should_Fail_When_FirstServiceTime_Without_TimeInterval_But_With_MileageInterval()
    {
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
        var result = await _validator.ValidateAsync(command);
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "");
    }

    [Fact]
    public async Task Should_Fail_When_FirstServiceMileage_Without_MileageInterval_But_With_TimeInterval()
    {
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
        var result = await _validator.ValidateAsync(command);
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "");
    }

    [Fact]
    public async Task Should_Pass_When_FirstService_Properties_Match_Available_Intervals()
    {
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
        var result = await _validator.ValidateAsync(command);
        Assert.True(result.IsValid);
    }

    #endregion

    #region Redundant Fields Robustness

    [Fact]
    public async Task Should_Pass_With_All_Optional_Fields_Provided_Time_Based()
    {
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
        var result = await _validator.ValidateAsync(command);
        Assert.True(result.IsValid);
    }

    [Fact]
    public async Task Should_Pass_With_All_Optional_Fields_Provided_Mileage_Based()
    {
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
        var result = await _validator.ValidateAsync(command);
        Assert.True(result.IsValid);
    }

    [Fact]
    public async Task Should_Pass_With_Maximum_Valid_Configuration()
    {
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
        var result = await _validator.ValidateAsync(command);
        Assert.True(result.IsValid);
    }

    #endregion

    #region Edge Case Boundary Testing

    [Fact]
    public async Task Should_Fail_When_TimeBuffer_Exactly_Equals_TimeInterval_Same_Units()
    {
        var command = CreateValidCommand(
            timeIntervalValue: 7,
            timeIntervalUnit: TimeUnitEnum.Days,
            timeBufferValue: 7,
            timeBufferUnit: TimeUnitEnum.Days,
            mileageInterval: null,
            mileageBuffer: null);
        var result = await _validator.ValidateAsync(command);
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "");
    }

    [Fact]
    public async Task Should_Pass_When_TimeBuffer_One_Less_Than_TimeInterval()
    {
        var command = CreateValidCommand(
            timeIntervalValue: 7,
            timeIntervalUnit: TimeUnitEnum.Days,
            timeBufferValue: 6,
            timeBufferUnit: TimeUnitEnum.Days,
            mileageInterval: null,
            mileageBuffer: null);
        var result = await _validator.ValidateAsync(command);
        Assert.True(result.IsValid);
    }

    [Fact]
    public async Task Should_Fail_When_MileageBuffer_Exactly_Equals_MileageInterval()
    {
        var command = CreateValidCommand(
            timeIntervalValue: null,
            timeIntervalUnit: null,
            mileageInterval: 5000,
            mileageBuffer: 5000);
        var result = await _validator.ValidateAsync(command);
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "");
    }

    [Fact]
    public async Task Should_Pass_When_MileageBuffer_One_Less_Than_MileageInterval()
    {
        var command = CreateValidCommand(
            timeIntervalValue: null,
            timeIntervalUnit: null,
            mileageInterval: 5000,
            mileageBuffer: 4999);
        var result = await _validator.ValidateAsync(command);
        Assert.True(result.IsValid);
    }

    #endregion

    #region Positive Edge Cases

    [Fact]
    public async Task Should_Pass_With_Shortest_Valid_Name()
    {
        var command = CreateValidCommand(
            name: "A",
            timeBufferValue: 1,
            timeBufferUnit: TimeUnitEnum.Days,
            mileageBuffer: 250);
        var result = await _validator.ValidateAsync(command);
        Assert.True(result.IsValid);
    }

    [Fact]
    public async Task Should_Pass_With_Unicode_Name()
    {
        var command = CreateValidCommand(
            name: "˜ˆç˙ø¬åß Âå††˙´∑ Íå∑",
            timeBufferValue: 1,
            timeBufferUnit: TimeUnitEnum.Days,
            mileageBuffer: 250);
        var result = await _validator.ValidateAsync(command);
        Assert.True(result.IsValid);
    }

    [Fact]
    public async Task Should_Pass_With_Hours_TimeUnit()
    {
        var command = CreateValidCommand(
            timeIntervalValue: 48,
            timeIntervalUnit: TimeUnitEnum.Hours,
            timeBufferValue: 1,
            timeBufferUnit: TimeUnitEnum.Hours,
            mileageInterval: null,
            mileageBuffer: null);
        var result = await _validator.ValidateAsync(command);
        Assert.True(result.IsValid);
    }

    [Fact]
    public async Task Should_Pass_With_Large_Interval_And_Buffer()
    {
        var command = CreateValidCommand(
            timeIntervalValue: 24,
            timeIntervalUnit: TimeUnitEnum.Days,
            timeBufferValue: 23,
            timeBufferUnit: TimeUnitEnum.Days,
            mileageInterval: null,
            mileageBuffer: null);
        var result = await _validator.ValidateAsync(command);
        Assert.True(result.IsValid);
    }

    #endregion

    #region Invalid Enum Tests

    [Fact]
    public async Task Should_Fail_When_TimeIntervalUnit_InvalidEnum()
    {
        var command = CreateValidCommand(
            timeIntervalValue: 5,
            timeIntervalUnit: (TimeUnitEnum)999, // invalid enum
            mileageInterval: null,
            mileageBuffer: null);
        var result = await _validator.ValidateAsync(command);
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "TimeIntervalUnit");
    }

    [Fact]
    public async Task Should_Fail_When_TimeBufferUnit_InvalidEnum()
    {
        var command = CreateValidCommand(
            timeIntervalValue: 5,
            timeIntervalUnit: TimeUnitEnum.Days,
            timeBufferValue: 1,
            timeBufferUnit: (TimeUnitEnum)999, // invalid enum
            mileageInterval: null,
            mileageBuffer: null);
        var result = await _validator.ValidateAsync(command);
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "TimeBufferUnit");
    }

    [Fact]
    public async Task Should_Fail_When_FirstServiceTimeUnit_InvalidEnum()
    {
        var command = CreateValidCommand(
            timeIntervalValue: 5,
            timeIntervalUnit: TimeUnitEnum.Days,
            firstServiceTimeValue: 1,
            firstServiceTimeUnit: (TimeUnitEnum)999);
        var result = await _validator.ValidateAsync(command);
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "FirstServiceTimeUnit");
    }

    #endregion
}