namespace Domain.Entities.Enums;

public enum WorkOrderStatusEnum
{
    CREATED = 1,
    ASSIGNED = 2,
    IN_PROGRESS = 3,
    WAITING_PARTS = 4,
    COMPLETED = 5,
    CANCELLED = 6,
    ON_HOLD = 7
}