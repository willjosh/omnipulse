using System;

using Application.Models.PaginationModels;

using Domain.Entities;

namespace Application.Contracts.Persistence;

public interface IInventoryRepository : IGenericRepository<Inventory>
{
    public Task<Inventory?> GetInventoryByItemIDAsync(int id);
    public PagedResult<Inventory> GetAllInventoriesPagedAsync(PaginationParameters parameters);
}