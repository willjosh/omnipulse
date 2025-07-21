using Application.Contracts.Persistence;

using Domain.Entities;

using Microsoft.EntityFrameworkCore;

using Persistence.DatabaseContext;

namespace Persistence.Repository;

public class FuelPurchasesRepository : GenericRepository<FuelPurchase>, IFuelPurchaseRepository
{
    public FuelPurchasesRepository(OmnipulseDatabaseContext context) : base(context) { }

    public async Task<bool> IsValidOdometerReading(int vehicleId, double odometerReading)
    {
        var vehicle = await _dbSet.FindAsync(vehicleId);
        return vehicle == null || odometerReading >= vehicle.OdometerReading;
    }

    public async Task<bool> IsReceiptNumberUniqueAsync(string receiptNumber)
    {
        return !await _dbSet.AnyAsync(fp => fp.ReceiptNumber == receiptNumber);
    }
}