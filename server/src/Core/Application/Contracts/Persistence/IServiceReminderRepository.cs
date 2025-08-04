using Domain.Entities;

namespace Application.Contracts.Persistence;

public interface IServiceReminderRepository : IGenericRepository<ServiceReminder>
{
    public Task<IReadOnlyList<ServiceReminder>> GetServiceRemindersByWorkOrderIdAsync(int workOrderId);
}