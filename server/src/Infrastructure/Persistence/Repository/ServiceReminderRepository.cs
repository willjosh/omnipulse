
using Application.Contracts.Persistence;

using Domain.Entities;

using Microsoft.EntityFrameworkCore;

using Persistence.DatabaseContext;

namespace Persistence.Repository;

public class ServiceReminderRepository : GenericRepository<ServiceReminder>, IServiceReminderRepository
{
    public ServiceReminderRepository(OmnipulseDatabaseContext context) : base(context) { }

    public async Task<IReadOnlyList<ServiceReminder>> GetServiceRemindersByWorkOrderIdAsync(int workOrderID)
    {
        return await _dbSet.Where(sr => sr.WorkOrderID == workOrderID).ToListAsync();
    }
}