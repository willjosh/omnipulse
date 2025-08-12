using Domain.Entities;

using MediatR;

namespace Application.Features.FuelPurchases.Command.DeleteFuelPurchase;

/// <summary>
/// Command to delete a <see cref="FuelPurchase"/> by ID.
/// </summary>
/// <param name="FuelPurchaseID">The ID of the Fuel Purchase to delete.</param>
/// <returns>The ID of the deleted <see cref="FuelPurchase"/>.</returns>
public record DeleteFuelPurchaseCommand(int FuelPurchaseID) : IRequest<int>;