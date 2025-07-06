using Domain.Entities.Enums;

using MediatR;

namespace Application.Features.ServiceTasks.Command.CreateServiceTask;

/// <summary>
/// Command for creating a new service task.
/// </summary>
/// <param name="Name">The name of the service task.</param>
/// <param name="Description">Optional description of the service task.</param>
/// <param name="EstimatedLabourHours">The estimated labour hours required for this service task.</param>
/// <param name="EstimatedCost">The estimated cost of performing this service task.</param>
/// <param name="Category">The category of the service task. See <see cref="ServiceTaskCategoryEnum"/> for options.</param>
/// <param name="IsActive">Whether the service task is active and available for use.</param>
/// <returns>The ID of the newly created service task.</returns>
/// <remarks>
/// This command implements the Command pattern using MediatR for creating service tasks.
/// It validates business rules such as unique name constraints and ensures proper data types.
/// </remarks>
/// <exception cref="Application.Exceptions.BadRequestException">
/// Thrown when validation fails for any of the input parameters.
/// </exception>
/// <exception cref="Application.Exceptions.DuplicateEntityException">
/// Thrown when a service task with the same name already exists.
/// </exception>
public record CreateServiceTaskCommand(
    string Name,
    string? Description,
    double EstimatedLabourHours,
    decimal EstimatedCost,
    ServiceTaskCategoryEnum Category,
    bool IsActive = true
) : IRequest<int>;