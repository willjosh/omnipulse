using System;

using Application.Models;
using Application.Models.PaginationModels;

using Domain.Entities;

namespace Application.Contracts.Persistence;

public interface IMaintenanceHistoryRepository : IGenericRepository<MaintenanceHistory>
{
    Task<PagedResult<MaintenanceHistory>> GetAllMaintenanceHistoriesPagedAsync(PaginationParameters parameters);
}