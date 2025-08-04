using System;

using Application.Models.PaginationModels;

using Domain.Entities;

namespace Application.Contracts.Persistence;

public interface IInventoryRepository : IGenericRepository<Inventory>
{
    public Task<Inventory?> GetInventoryByItemIDAsync(int id);
    public Task<PagedResult<Inventory>> GetAllInventoriesPagedAsync(PaginationParameters parameters);
    public Task<Inventory?> GetInventoryWithDetailsAsync(int id);
}