using Application.Contracts.Logger;
using Application.Contracts.Persistence;
using Application.Exceptions;

using AutoMapper;

using Domain.Entities;

using MediatR;

namespace Application.Features.ServiceSchedules.Command.DeleteServiceSchedule;

public class DeleteServiceScheduleCommandHandler : IRequestHandler<DeleteServiceScheduleCommand, int>
{
    private readonly IServiceScheduleRepository _serviceScheduleRepository;
    private readonly IAppLogger<DeleteServiceScheduleCommandHandler> _logger;

    public DeleteServiceScheduleCommandHandler(IServiceScheduleRepository serviceScheduleRepository, IAppLogger<DeleteServiceScheduleCommandHandler> logger)
    {
        _serviceScheduleRepository = serviceScheduleRepository;
        _logger = logger;
    }

    public async Task<int> Handle(DeleteServiceScheduleCommand request, CancellationToken cancellationToken)
    {
        // Validate ServiceScheduleID
        _logger.LogInformation($"Validating ServiceSchedule with ID: {request.ServiceScheduleID}");
        var serviceSchedule = await _serviceScheduleRepository.GetByIdAsync(request.ServiceScheduleID);
        if (serviceSchedule == null)
        {
            _logger.LogError($"ServiceSchedule with ID {request.ServiceScheduleID} not found.");
            throw new EntityNotFoundException(typeof(ServiceSchedule).ToString(), "ID", request.ServiceScheduleID.ToString());
        }

        // Delete ServiceSchedule
        _serviceScheduleRepository.Delete(serviceSchedule);

        // Save Changes
        await _serviceScheduleRepository.SaveChangesAsync();
        _logger.LogInformation($"ServiceSchedule with ID: {request.ServiceScheduleID} deleted");

        return request.ServiceScheduleID;
    }
}