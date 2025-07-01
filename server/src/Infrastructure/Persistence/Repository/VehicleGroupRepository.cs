using System;
using Application.Contracts.Persistence;
using Application.Models;
using Application.Models.PaginationModels;
using Domain.Entities;
using Persistence.DatabaseContext;

namespace Persistence.Repository;

public class VehicleGroupRepository : GenericRepository<VehicleGroup>, IVehicleGroupRepository
{
    public VehicleGroupRepository(OmnipulseDatabaseContext context) : base(context) { }

    public Task<PagedResult<VehicleGroup>> GetAllVehicleGroupsPagedAsync(PaginationParameters parameters)
    {
        throw new NotImplementedException();
    }
}
