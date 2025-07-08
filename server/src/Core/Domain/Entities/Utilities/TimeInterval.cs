namespace Domain.Entities.Enums;

public class TimeInterval
{
    public int Value { get; }
    public TimeUnitEnum Unit { get; }

    public TimeInterval(int value, TimeUnitEnum unit)
    {
        if (value <= 0) throw new ArgumentException("Time interval value must be positive");
        Value = value;
        Unit = unit;
    }

    public DateTime AddTo(DateTime baseTime)
    {
        return Unit switch
        {
            TimeUnitEnum.Hours => baseTime.AddHours(Value),
            TimeUnitEnum.Days => baseTime.AddDays(Value),
            TimeUnitEnum.Weeks => baseTime.AddDays(Value * 7),
            TimeUnitEnum.Months => baseTime.AddMonths(Value),
            _ => throw new NotSupportedException($"Unsupported Unit: {Unit}")
        };
    }
}
