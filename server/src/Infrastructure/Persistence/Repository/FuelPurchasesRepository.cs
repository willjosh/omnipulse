using Application.Contracts.Persistence;

using Domain.Entities;
using Domain.Entities.Enums;

using Microsoft.EntityFrameworkCore;

using Persistence.DatabaseContext;

namespace Persistence.Repository;

public class FuelPurchasesRepository : GenericRepository<FuelPurchase>, IFuelPurchaseRepository
{
    public FuelPurchasesRepository(OmnipulseDatabaseContext context) : base(context) { }

    public async Task<bool> IsValidOdometerReading(int vehicleId, double OdometerReading)
    {
        var vehicle = await _dbSet.FindAsync(vehicleId);
        if (vehicle != null)
        {
            return OdometerReading >= vehicle.OdometerReading;
        }
        return true;
    }
}