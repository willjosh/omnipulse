using Application.Contracts.Persistence;
using Application.Models.PaginationModels;

using Domain.Entities;

using Microsoft.EntityFrameworkCore;

using Persistence.DatabaseContext;

namespace Persistence.Repository;

public class InventoryRepository : GenericRepository<Inventory>, IInventoryRepository
{
    public InventoryRepository(OmnipulseDatabaseContext context) : base(context)
    {
    }

    public PagedResult<Inventory> GetAllInventoriesPagedAsync(PaginationParameters parameters)
    {
        throw new NotImplementedException();
    }

    public async Task<Inventory?> GetInventoryByItemIDAsync(int id)
    {
        return await _dbSet.FirstOrDefaultAsync(i => i.InventoryItemID == id);
    }
}