using Domain.Entities;

namespace Application.Contracts.Persistence;

public interface IXrefServiceScheduleServiceTaskRepository
{
    /// <summary>
    /// Gets all xref records for a given <see cref="ServiceSchedule"/>.
    /// </summary>
    /// <param name="serviceScheduleId">The ID of the service schedule.</param>
    /// <returns>A list of xref records associated with the specified service schedule.</returns>
    Task<List<XrefServiceScheduleServiceTask>> GetByServiceScheduleIdAsync(int serviceScheduleId);

    /// <summary>
    /// Gets all xref records for the given list of <see cref="ServiceSchedule"/> IDs.
    /// </summary>
    /// <param name="serviceScheduleIds">A collection of service schedule IDs.</param>
    /// <returns>A list of xref records associated with the specified service schedules.</returns>
    Task<List<XrefServiceScheduleServiceTask>> GetByServiceScheduleIdsAsync(IEnumerable<int> serviceScheduleIds);

    /// <summary>
    /// Gets all xref records for a given <see cref="ServiceTask"/>.
    /// </summary>
    /// <param name="serviceTaskId">The ID of the service task.</param>
    /// <returns>A list of xref records associated with the specified service task.</returns>
    Task<List<XrefServiceScheduleServiceTask>> GetByServiceTaskIdAsync(int serviceTaskId);

    /// <summary>
    /// Gets all xref records for the given list of <see cref="ServiceTask"/> IDs.
    /// </summary>
    /// <param name="serviceTaskIds">A collection of service task IDs.</param>
    /// <returns>A list of xref records associated with the specified service tasks.</returns>
    Task<List<XrefServiceScheduleServiceTask>> GetByServiceTaskIdsAsync(IEnumerable<int> serviceTaskIds);

    /// <summary>
    /// Checks if a link exists between a service schedule and a service task.
    /// </summary>
    /// <param name="serviceScheduleId">The ID of the service schedule.</param>
    /// <param name="serviceTaskId">The ID of the service task.</param>
    /// <returns>True if the link exists; otherwise, false.</returns>
    Task<bool> ExistsAsync(int serviceScheduleId, int serviceTaskId);

    /// <summary>
    /// Adds a new link between a service schedule and a service task.
    /// </summary>
    /// <param name="xref">The xref entity to add.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task AddAsync(XrefServiceScheduleServiceTask xref);

    /// <summary>
    /// Removes a link between a service schedule and a service task.
    /// </summary>
    /// <param name="serviceScheduleId">The ID of the service schedule.</param>
    /// <param name="serviceTaskId">The ID of the service task.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task RemoveAsync(int serviceScheduleId, int serviceTaskId);
}