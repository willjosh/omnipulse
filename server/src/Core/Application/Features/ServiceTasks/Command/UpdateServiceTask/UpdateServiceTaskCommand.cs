using Domain.Entities.Enums;

using MediatR;

namespace Application.Features.ServiceTasks.Command.UpdateServiceTask;

/// <summary>
/// Command for updating an existing service task.
/// </summary>
/// <param name="ServiceTaskID">The ID of the service task to update.</param>
/// <param name="Name">The updated name of the service task.</param>
/// <param name="Description">The updated description of the service task.</param>
/// <param name="EstimatedLabourHours">The updated estimated labour hours required for this service task.</param>
/// <param name="EstimatedCost">The updated estimated cost of performing this service task.</param>
/// <param name="Category">The updated category of the service task. See <see cref="ServiceTaskCategoryEnum"/> for options.</param>
/// <param name="IsActive">Whether the service task is active and available for use.</param>
/// <returns>The ID of the updated service task.</returns>
/// <exception cref="Application.Exceptions.BadRequestException">
/// Thrown when validation fails for any of the input parameters.
/// </exception>
/// <exception cref="Application.Exceptions.EntityNotFoundException">
/// Thrown when the service task with the specified ID does not exist.
/// </exception>
/// <exception cref="Application.Exceptions.DuplicateEntityException">
/// Thrown when a service task with the same name already exists (excluding the current service task).
/// </exception>
public record UpdateServiceTaskCommand(
    int ServiceTaskID,
    string Name,
    string? Description,
    double EstimatedLabourHours,
    decimal EstimatedCost,
    ServiceTaskCategoryEnum Category,
    bool IsActive
) : IRequest<int>;