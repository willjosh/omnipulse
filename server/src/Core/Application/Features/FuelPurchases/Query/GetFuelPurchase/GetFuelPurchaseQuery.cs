using Domain.Entities;

using MediatR;

namespace Application.Features.FuelPurchases.Query.GetFuelPurchase;

/// <summary>
/// Query to get a single <see cref="FuelPurchase"/> by ID.
/// </summary>
/// <param name="FuelPurchaseID">The ID of the <see cref="FuelPurchase"/> to retrieve.</param>
/// <returns>The <see cref="FuelPurchaseDTO"/> containing the <see cref="FuelPurchase"/> details.</returns>
public sealed record GetFuelPurchaseQuery(int FuelPurchaseID) : IRequest<FuelPurchaseDTO>;