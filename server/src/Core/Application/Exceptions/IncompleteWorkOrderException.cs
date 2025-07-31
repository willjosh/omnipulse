public class IncompleteWorkOrderException : Exception
{
    public IncompleteWorkOrderException(int WorkOrderID) : base($"WorkOrder with ID: {WorkOrderID} is not completed") { }
}