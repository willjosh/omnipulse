using Application.Models.PaginationModels;

using Domain.Entities;

namespace Application.Contracts.Persistence;

public interface IVehicleRepository : IGenericRepository<Vehicle>
{
    public Task<bool> VinExistAsync(string VIN);
    public Task<bool> LicensePlateExistAsync(string licensePlate);
    public Task VehicleDeactivateAsync(int VehicleID);
    public Task<Vehicle?> GetVehicleWithDetailsAsync(int vehicleId);
    public Task<PagedResult<Vehicle>> GetAllVehiclesPagedAsync(PaginationParameters parameters);
    public Task<int> GetAllAssignedVehicleAsync();
    public Task<int> GetAllUnassignedVehicleAsync();
    public Task<int> GetActiveVehicleCountAsync();
    public Task<int> GetMaintenanceVehicleCountAsync();
    public Task<int> GetOutOfServiceVehicleCountAsync();
    public Task<int> GetInactiveVehicleCountAsync();
}