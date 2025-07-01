using System;

using Application.Models;
using Application.Models.PaginationModels;

using Domain.Entities;

namespace Application.Contracts.Persistence;

public interface IVehicleGroupRepository : IGenericRepository<VehicleGroup>
{
    public Task<PagedResult<VehicleGroup>> GetAllVehicleGroupsPagedAsync(PaginationParameters parameters);
}