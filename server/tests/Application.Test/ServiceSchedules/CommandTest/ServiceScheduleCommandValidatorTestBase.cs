using Application.Test.ServiceSchedules.CommandTest.CreateServiceSchedule;
using Application.Test.ServiceSchedules.CommandTest.UpdateServiceSchedule;

using Domain.Entities.Enums;

using FluentValidation;

namespace Application.Test.ServiceSchedules.CommandTest;

/// <summary>
/// Base validator test for <see cref="CreateServiceScheduleCommandValidatorTest"/> and <see cref="UpdateServiceScheduleCommandValidatorTest"/>.
/// </summary>
/// <typeparam name="TCommand">The type of command to test.</typeparam>
/// <typeparam name="TValidator">The type of validator to test. Must be <see cref="IValidator{TCommand}"/>.</typeparam>
public abstract class ServiceScheduleCommandValidatorTestBase<TCommand, TValidator>
    where TValidator : IValidator<TCommand>
{
    protected abstract TValidator Validator { get; }

    protected abstract TCommand CreateValidCommand(
        int serviceProgramId = 1,
        string name = "5000 km / 6 week service",
        List<int>? serviceTaskIDs = null,
        int? timeIntervalValue = 6,
        TimeUnitEnum? timeIntervalUnit = TimeUnitEnum.Weeks,
        int? timeBufferValue = 1,
        TimeUnitEnum? timeBufferUnit = TimeUnitEnum.Days,
        int? mileageInterval = 5000,
        int? mileageBuffer = 250,
        DateTime? firstServiceDate = null,
        int? firstServiceMileage = null,
        bool isActive = true);

    [Fact]
    public void Validator_Should_Not_Be_Null()
    {
        Assert.NotNull(Validator);
    }

    [Trait("Category", "Positive")]
    [Fact]
    public async Task Validator_Should_Pass_With_Valid_Command()
    {
        // Arrange
        var command = CreateValidCommand(serviceTaskIDs: [1]);

        // Act
        var result = await Validator.ValidateAsync(command);

        // Assert
        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }

    // [Fact]
    // public async Task Validator_Should_Fail_When_ServiceTaskIDs_Is_Null()
    // {
    //     // Arrange
    //     var command = CreateValidCommand(serviceTaskIDs: null);

    //     // Act
    //     var result = await Validator.ValidateAsync(command);

    //     // Assert
    //     Assert.False(result.IsValid);
    //     Assert.Contains(result.Errors, e => e.PropertyName == "ServiceTaskIDs");
    // }

    [Fact]
    public async Task Validator_Should_Fail_When_ServiceTaskIDs_Is_Empty()
    {
        // Arrange
        var command = CreateValidCommand(serviceTaskIDs: []);

        // Act
        var result = await Validator.ValidateAsync(command);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "ServiceTaskIDs");
    }

    [Trait("Category", "ServiceProgramID")]
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

    [Trait("Category", "Name")]
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

    [Trait("Category", "Name")]
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

    [Trait("Category", "Name")]
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

    [Trait("Category", "TimeIntervalValue")]
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

    [Trait("Category", "TimeIntervalValue")]
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

    [Trait("Category", "TimeIntervalUnit")]
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

    [Trait("Category", "TimeIntervalUnit")]
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

    [Trait("Category", "MileageInterval")]
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

    [Trait("Category", "Time")]
    [Theory]
    [InlineData(5, 5, false)] // Equal
    [InlineData(5, 4, true)]  // Buffer < Interval
    [InlineData(5, 6, false)] // Buffer > Interval
    public async Task Validator_Should_Handle_TimeBuffer_Comparisons(int interval, int buffer, bool isValid)
    {
        // Arrange
        var command = CreateValidCommand(
            timeIntervalValue: interval,
            timeIntervalUnit: TimeUnitEnum.Days,
            timeBufferValue: buffer,
            timeBufferUnit: TimeUnitEnum.Days);

        // Act
        var result = await Validator.ValidateAsync(command);

        // Assert
        Assert.Equal(isValid, result.IsValid);
    }

    [Trait("Category", "Mileage")]
    [Theory]
    [InlineData(1000, 1000, false)] // Equal
    [InlineData(1000, 999, true)]   // Buffer < Interval
    [InlineData(1000, 1001, false)] // Buffer > Interval
    public async Task Validator_Should_Handle_MileageBuffer_Comparisons(int interval, int buffer, bool isValid)
    {
        // Arrange
        var command = CreateValidCommand(
            timeIntervalValue: null,
            timeIntervalUnit: null,
            timeBufferValue: null,
            timeBufferUnit: null,
            mileageInterval: interval,
            mileageBuffer: buffer);

        // Act
        var result = await Validator.ValidateAsync(command);

        // Assert
        Assert.Equal(isValid, result.IsValid);
    }

    [Trait("Category", "FirstService")]
    [Theory]
    [InlineData(true, true, true, true)]     // All valid
    [InlineData(false, true, true, false)]   // Missing time interval
    [InlineData(true, false, true, false)]   // Missing mileage interval
    [InlineData(false, false, false, false)] // Missing all intervals
    public async Task Validator_Should_Handle_FirstService_Dependencies(
        bool hasTimeInterval,
        bool hasMileageInterval,
        bool hasFirstService,
        bool isValid)
    {
        // Arrange
        var command = CreateValidCommand(
            timeIntervalValue: hasTimeInterval ? 6 : null,
            timeIntervalUnit: hasTimeInterval ? TimeUnitEnum.Days : null,
            mileageInterval: hasMileageInterval ? 5000 : null,
            firstServiceDate: hasFirstService ? 3 : null,
            firstServiceDate: hasFirstService ? TimeUnitEnum.Days : null,
            firstServiceMileage: hasFirstService ? 2500 : null);

        // Act
        var result = await Validator.ValidateAsync(command);

        // Assert
        Assert.Equal(isValid, result.IsValid);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(-100)]
    public async Task Validator_Should_Fail_When_FirstServiceDate_Is_Negative(int invalidValue)
    {
        // Arrange

        // Act
        var result = await Validator.ValidateAsync(command);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "FirstServiceDate");
    }

    [Fact]
    public async Task Validator_Should_Pass_When_FirstServiceDate_Is_Zero()
    {
        // Arrange

        // Act
        var result = await Validator.ValidateAsync(command);

        // Assert
        Assert.True(result.IsValid);
    }

    [Fact]
    public async Task Validator_Should_Fail_When_FirstServiceDate_Without_Unit()
    {
        // Arrange

        // Act
        var result = await Validator.ValidateAsync(command);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "");
    }

    [Fact]
    public async Task Validator_Should_Fail_When_FirstServiceDate_Without_Value()
    {
        // Arrange

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
            firstServiceDate: 2,
            firstServiceDate: TimeUnitEnum.Days);

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
            firstServiceDate: 3,
            firstServiceDate: TimeUnitEnum.Days,
            firstServiceMileage: 2500);

        // Act
        var result = await Validator.ValidateAsync(command);

        // Assert
        Assert.True(result.IsValid);
    }

    [Trait("Category", "TimeUnit")]
    [Fact]
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

    [Trait("Category", "TimeUnit")]
    [Fact]
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

    [Trait("Category", "Robustness")]
    [Fact]
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
            firstServiceDate: 6,
            firstServiceDate: TimeUnitEnum.Days,
            firstServiceMileage: null);

        // Act
        var result = await Validator.ValidateAsync(command);

        // Assert
        Assert.True(result.IsValid);
    }

    [Trait("Category", "Robustness")]
    [Fact]
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
            firstServiceDate: null,
            firstServiceDate: null,
            firstServiceMileage: 5000);

        // Act
        var result = await Validator.ValidateAsync(command);

        // Assert
        Assert.True(result.IsValid);
    }

    [Trait("Category", "Robustness")]
    [Fact]
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
            firstServiceDate: 6,
            firstServiceDate: TimeUnitEnum.Days,
            firstServiceMileage: 5000);

        // Act
        var result = await Validator.ValidateAsync(command);

        // Assert
        Assert.True(result.IsValid);
    }

    [Trait("Category", "EdgeCase")]
    [Fact]
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

    [Trait("Category", "EdgeCase")]
    [Fact]
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

    [Trait("Category", "Unicode")]
    [Fact]
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

    [Trait("Category", "Boundary")]
    [Fact]
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

    [Trait("Category", "EnumValidation")]
    [Fact]
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

    [Trait("Category", "EnumValidation")]
    [Fact]
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

    [Trait("Category", "EnumValidation")]
    [Fact]
    public async Task Validator_Should_Fail_When_FirstServiceDate_InvalidEnum()
    {
        // Arrange
        var command = CreateValidCommand(
            timeIntervalValue: 5,
            timeIntervalUnit: TimeUnitEnum.Days,
            firstServiceDate: 1,
            firstServiceDate: (TimeUnitEnum)999);

        // Act
        var result = await Validator.ValidateAsync(command);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "FirstServiceDate");
    }

    [Trait("Category", "FirstServiceEdge")]
    [Fact]
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
            firstServiceDate: 3,
            firstServiceDate: TimeUnitEnum.Days,
            firstServiceMileage: 1000);

        // Act
        var result = await Validator.ValidateAsync(command);

        // Assert
        Assert.False(result.IsValid);
        // Should fail for multiple reasons: no intervals provided AND FirstService requires intervals
        Assert.Contains(result.Errors, e => e.PropertyName == "");
    }

    [Trait("Category", "FirstServiceEdge")]
    [Fact]
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
            firstServiceDate: 3,
            firstServiceDate: TimeUnitEnum.Days,
            firstServiceMileage: null);

        // Act
        var result = await Validator.ValidateAsync(command);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "");
    }

    [Trait("Category", "FirstServiceEdge")]
    [Fact]
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
            firstServiceDate: null,
            firstServiceDate: null,
            firstServiceMileage: 1000);

        // Act
        var result = await Validator.ValidateAsync(command);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "");
    }

    [Trait("Category", "FirstServiceEdge")]
    [Fact]
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
            firstServiceDate: 3,
            firstServiceDate: TimeUnitEnum.Days,
            firstServiceMileage: 2500);

        // Act
        var result = await Validator.ValidateAsync(command);

        // Assert
        Assert.True(result.IsValid);
    }

    [Trait("Category", "FirstServiceEdge")]
    [Fact]
    public async Task Validator_Should_Pass_With_FirstService_Equal_To_Interval()
    {
        // Arrange
        var command = CreateValidCommand(
            timeIntervalValue: 6,
            timeIntervalUnit: TimeUnitEnum.Days,
            timeBufferValue: null,
            timeBufferUnit: null,
            firstServiceDate: 6,
            firstServiceDate: TimeUnitEnum.Days,
            mileageInterval: 5000,
            firstServiceMileage: 5000);

        // Act
        var result = await Validator.ValidateAsync(command);

        // Assert
        Assert.True(result.IsValid);
    }

    [Trait("Category", "FirstServiceEdge")]
    [Fact]
    public async Task Validator_Should_Pass_With_FirstService_Greater_Than_Interval()
    {
        // Arrange
        var command = CreateValidCommand(
            timeIntervalValue: 6,
            timeIntervalUnit: TimeUnitEnum.Days,
            timeBufferValue: null,
            timeBufferUnit: null,
            firstServiceDate: 12,
            firstServiceDate: TimeUnitEnum.Days,
            mileageInterval: 5000,
            firstServiceMileage: 10000);

        // Act
        var result = await Validator.ValidateAsync(command);

        // Assert
        Assert.True(result.IsValid);
    }

    [Trait("Category", "PositiveScenario")]
    [Fact]
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

    [Trait("Category", "PositiveScenario")]
    [Fact]
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

    [Trait("Category", "EdgeCase")]
    [Fact]
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

    [Trait("Category", "EdgeCase")]
    [Fact]
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

    [Trait("Category", "General")]
    [Fact]
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