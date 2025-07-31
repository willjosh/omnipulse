using Application.Contracts.Persistence;

using Domain.Entities;

using Microsoft.EntityFrameworkCore;

using Persistence.DatabaseContext;

namespace Persistence.Repository;

public class InventoryRepository : GenericRepository<Inventory>, IInventoryRepository
{
    public InventoryRepository(OmnipulseDatabaseContext context) : base(context)
    {
    }

    public async Task<Inventory?> GetInventoryByItemIDAsync(int id)
    {
        return await _dbSet.FirstOrDefaultAsync(i => i.InventoryItemID == id);
    }
}