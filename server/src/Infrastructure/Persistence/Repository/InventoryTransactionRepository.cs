using Application.Contracts.Persistence;

using Domain.Entities;

using Persistence.DatabaseContext;

namespace Persistence.Repository;

public class InventoryTransactionRepository : GenericRepository<InventoryTransaction>, IInventoryTransactionRepository
{
    public InventoryTransactionRepository(OmnipulseDatabaseContext context) : base(context)
    {
    }
}