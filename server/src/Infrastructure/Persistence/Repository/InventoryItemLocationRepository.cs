using Application.Contracts.Persistence;

using Domain.Entities;

using Persistence.DatabaseContext;

namespace Persistence.Repository;

public class InventoryItemLocationRepository : GenericRepository<InventoryItemLocation>, IInventoryItemLocationRepository
{
    public InventoryItemLocationRepository(OmnipulseDatabaseContext context) : base(context) { }
}