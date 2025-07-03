using System;

using Domain.Entities;

namespace Application.Contracts.Persistence;

public interface IFuelPurchaseRepository : IGenericRepository<FuelPurchase>
{
    public Task<bool> IsValidOdometerReading(int vehicleId, double OdometerReading);
}