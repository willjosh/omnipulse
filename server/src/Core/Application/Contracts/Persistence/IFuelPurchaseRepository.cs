using Application.Models.PaginationModels;

using Domain.Entities;

namespace Application.Contracts.Persistence;

public interface IFuelPurchaseRepository : IGenericRepository<FuelPurchase>
{
    /// <summary>
    /// Validates if the odometer reading is greater than the last recorded reading for the vehicle.
    /// </summary>
    /// <param name="vehicleId">The vehicle ID</param>
    /// <param name="odometerReading">The new odometer reading</param>
    /// <returns>True if the reading is valid (greater than previous), false otherwise</returns>
    Task<bool> IsValidOdometerReading(int vehicleId, double odometerReading);

    /// <summary>
    /// Checks if a receipt number is unique in the system.
    /// </summary>
    /// <param name="receiptNumber">The receipt number to check</param>
    /// <returns>True if unique, false if already exists</returns>
    Task<bool> IsReceiptNumberUniqueAsync(string receiptNumber);

    public Task<PagedResult<FuelPurchase>> GetAllFuelPurchasesPagedAsync(PaginationParameters parameters);
}