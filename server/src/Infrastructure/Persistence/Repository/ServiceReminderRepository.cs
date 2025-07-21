using Application.Contracts.Persistence;

using Domain.Entities;

using Persistence.DatabaseContext;

namespace Persistence.Repository;

public class ServiceReminderRepository : GenericRepository<ServiceReminder>, IServiceReminderRepository
{
    public ServiceReminderRepository(OmnipulseDatabaseContext context) : base(context) { }
}