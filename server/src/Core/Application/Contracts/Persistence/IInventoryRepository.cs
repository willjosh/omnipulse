using System;

using Domain.Entities;

namespace Application.Contracts.Persistence;

public interface IInventoryRepository : IGenericRepository<Inventory>
{
    public Task<Inventory?> GetInventoryByItemIDAsync(int id);
}