using Application.Contracts.Logger;
using Application.Contracts.Persistence;
using Application.Contracts.Services;
using Domain.Entities;
using Domain.Entities.Enums;

namespace Application.Services;

public class ServiceReminderStatusUpdater : IServiceReminderStatusUpdater
{
    private readonly IServiceReminderRepository _serviceReminderRepository;
    private readonly IVehicleRepository _vehicleRepository;
    private readonly IAppLogger<ServiceReminderStatusUpdater> _logger;

    public ServiceReminderStatusUpdater(
        IServiceReminderRepository serviceReminderRepository,
        IVehicleRepository vehicleRepository,
        IAppLogger<ServiceReminderStatusUpdater> logger)
    {
        _serviceReminderRepository = serviceReminderRepository;
        _vehicleRepository = vehicleRepository;
        _logger = logger;
    }

    public async Task UpdateAllReminderStatusesAsync(CancellationToken cancellationToken = default)
    {
    }
}
