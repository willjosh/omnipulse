public class WorkOrderAlreadyCompletedException : Exception
{
    public WorkOrderAlreadyCompletedException(int workOrderId)
        : base($"Work order {workOrderId} is already completed") { }
}