using System;
using Domain.Entities;

namespace Application.Contracts.Persistence;

public interface IVehicleRepository : IGenericRepository<Vehicle>
{
    public Task<bool> VinExistAsync(string VIN);
}
