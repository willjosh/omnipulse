using Domain.Entities.Enums;

namespace Application.Features.ServiceReminders.Query;

/// <summary>
/// Extension methods for ServiceReminderDTO to provide schedule-based functionality.
/// </summary>
public static class ServiceReminderDTOExtensions
{
    /// <summary>
    /// Gets all task names as a comma-separated string.
    /// </summary>
    /// <param name="reminder">The service reminder DTO.</param>
    /// <returns>Comma-separated list of task names.</returns>
    public static string GetTaskNamesList(this ServiceReminderDTO reminder)
    {
        return string.Join(", ", reminder.ServiceTasks.Select(t => t.ServiceTaskName));
    }

    /// <summary>
    /// Gets all task categories represented in this reminder.
    /// </summary>
    /// <param name="reminder">The service reminder DTO.</param>
    /// <returns>Distinct list of task categories.</returns>
    public static List<ServiceTaskCategoryEnum> GetTaskCategories(this ServiceReminderDTO reminder)
    {
        return reminder.ServiceTasks.Select(t => t.ServiceTaskCategory).Distinct().ToList();
    }

    /// <summary>
    /// Checks if this reminder contains any required tasks.
    /// </summary>
    /// <param name="reminder">The service reminder DTO.</param>
    /// <returns>True if any task is marked as required.</returns>
    public static bool HasRequiredTasks(this ServiceReminderDTO reminder)
    {
        return reminder.ServiceTasks.Any(t => t.IsRequired);
    }

    /// <summary>
    /// Gets tasks filtered by category.
    /// </summary>
    /// <param name="reminder">The service reminder DTO.</param>
    /// <param name="category">The category to filter by.</param>
    /// <returns>Tasks matching the specified category.</returns>
    public static List<ServiceTaskInfoDTO> GetTasksByCategory(this ServiceReminderDTO reminder, ServiceTaskCategoryEnum category)
    {
        return reminder.ServiceTasks.Where(t => t.ServiceTaskCategory == category).ToList();
    }

    /// <summary>
    /// Gets the most expensive task in this reminder.
    /// </summary>
    /// <param name="reminder">The service reminder DTO.</param>
    /// <returns>The task with the highest estimated cost, or null if no tasks.</returns>
    public static ServiceTaskInfoDTO? GetMostExpensiveTask(this ServiceReminderDTO reminder)
    {
        return reminder.ServiceTasks.OrderByDescending(t => t.EstimatedCost).FirstOrDefault();
    }

    /// <summary>
    /// Gets the task requiring the most time in this reminder.
    /// </summary>
    /// <param name="reminder">The service reminder DTO.</param>
    /// <returns>The task with the highest estimated labour hours, or null if no tasks.</returns>
    public static ServiceTaskInfoDTO? GetMostTimeConsumingTask(this ServiceReminderDTO reminder)
    {
        return reminder.ServiceTasks.OrderByDescending(t => t.EstimatedLabourHours).FirstOrDefault();
    }

    /// <summary>
    /// Calculates the average cost per task.
    /// </summary>
    /// <param name="reminder">The service reminder DTO.</param>
    /// <returns>Average cost per task, or 0 if no tasks.</returns>
    public static decimal GetAverageCostPerTask(this ServiceReminderDTO reminder)
    {
        return reminder.TaskCount > 0 ? reminder.TotalEstimatedCost / reminder.TaskCount : 0;
    }

    /// <summary>
    /// Calculates the average time per task.
    /// </summary>
    /// <param name="reminder">The service reminder DTO.</param>
    /// <returns>Average labour hours per task, or 0 if no tasks.</returns>
    public static double GetAverageTimePerTask(this ServiceReminderDTO reminder)
    {
        return reminder.TaskCount > 0 ? reminder.TotalEstimatedLabourHours / reminder.TaskCount : 0;
    }

    /// <summary>
    /// Gets a detailed breakdown of tasks by category with totals.
    /// </summary>
    /// <param name="reminder">The service reminder DTO.</param>
    /// <returns>Dictionary with category as key and summary info as value.</returns>
    public static Dictionary<ServiceTaskCategoryEnum, (int Count, decimal TotalCost, double TotalHours)> GetCategoryBreakdown(this ServiceReminderDTO reminder)
    {
        return reminder.ServiceTasks
            .GroupBy(t => t.ServiceTaskCategory)
            .ToDictionary(
                g => g.Key,
                g => (
                    Count: g.Count(),
                    TotalCost: g.Sum(t => t.EstimatedCost),
                    TotalHours: g.Sum(t => t.EstimatedLabourHours)
                )
            );
    }

    /// <summary>
    /// Groups tasks by their category for organized display.
    /// </summary>
    /// <param name="reminder">The service reminder DTO.</param>
    /// <returns>Tasks organized by category.</returns>
    public static Dictionary<ServiceTaskCategoryEnum, List<ServiceTaskInfoDTO>> GetTasksByCategories(this ServiceReminderDTO reminder)
    {
        return reminder.ServiceTasks
            .GroupBy(t => t.ServiceTaskCategory)
            .ToDictionary(g => g.Key, g => g.ToList());
    }
}