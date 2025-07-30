namespace Domain.Entities;

public class InspectionPassFailItem : InspectionItemBase
{
    public required bool Passed { get; set; } = false;
}