using Application.Contracts.Persistence;
using Application.Models;
using Application.Models.PaginationModels;
using Domain.Entities;
using Domain.Entities.Enums;
using Microsoft.EntityFrameworkCore;
using Persistence.DatabaseContext;

namespace Persistence.Repository;

public class VehicleRepository : GenericRepository<Vehicle>, IVehicleRepository
{
    public VehicleRepository(OmnipulseDatabaseContext context) : base(context) { }

    public Task<PagedResult<Vehicle>> GetAllVehiclesPagedAsync(PaginationParameters parameters)
    {
        throw new NotImplementedException();
    }

    public async Task<Vehicle?> GetVehicleWithDetailsAsync(int vehicleId)
    {
        return await _dbSet
            .Include(v => v.User)
            .Include(v => v.VehicleGroup)
            .FirstOrDefaultAsync(v => v.ID == vehicleId);
    }

    public Task<bool> LicensePlateExistAsync(string licensePlate)
    {
        return _dbSet.AnyAsync(v => v.LicensePlate == licensePlate);
    }

    public async Task VehicleDeactivateAsync(int VehicleID)
    {
        var vehicle = await _dbSet.FindAsync(VehicleID);
        if (vehicle != null)
        {
            vehicle.Status = VehicleStatusEnum.INACTIVE;
        }
    }

    public async Task<bool> VinExistAsync(string VIN)
    {
        return await _dbSet.AnyAsync(v => v.VIN == VIN);
    }
}
