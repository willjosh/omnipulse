using Application.Contracts.Persistence;

using Domain.Entities;

using Persistence.DatabaseContext;

namespace Persistence.Repository;

public class InventoryRepository : GenericRepository<Inventory>, IInventoryRepository
{
    public InventoryRepository(OmnipulseDatabaseContext context) : base(context)
    {
    }
}