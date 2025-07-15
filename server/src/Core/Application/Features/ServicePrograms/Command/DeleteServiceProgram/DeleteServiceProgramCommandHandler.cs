using Application.Contracts.Logger;
using Application.Contracts.Persistence;
using Application.Exceptions;

using Domain.Entities;

using MediatR;

namespace Application.Features.ServicePrograms.Command.DeleteServiceProgram;

public class DeleteServiceProgramCommandHandler : IRequestHandler<DeleteServiceProgramCommand, int>
{
    private readonly IServiceProgramRepository _serviceProgramRepository;
    private readonly IServiceScheduleRepository _serviceScheduleRepository;
    private readonly IAppLogger<DeleteServiceProgramCommandHandler> _logger;

    public DeleteServiceProgramCommandHandler(
        IServiceProgramRepository serviceProgramRepository,
        IServiceScheduleRepository serviceScheduleRepository,
        IAppLogger<DeleteServiceProgramCommandHandler> logger)
    {
        _serviceProgramRepository = serviceProgramRepository;
        _serviceScheduleRepository = serviceScheduleRepository;
        _logger = logger;
    }

    public async Task<int> Handle(DeleteServiceProgramCommand request, CancellationToken cancellationToken)
    {
        // Get ServiceProgram by ID
        var targetServiceProgram = await _serviceProgramRepository.GetByIdAsync(request.ServiceProgramID);
        if (targetServiceProgram == null)
        {
            _logger.LogError($"{nameof(ServiceProgram)} with {nameof(ServiceProgram.ID)} {request.ServiceProgramID} not found.");
            throw new EntityNotFoundException(nameof(ServiceProgram), nameof(ServiceProgram.ID), request.ServiceProgramID.ToString());
        }

        // Delete all Service Schedules inside the Service Program

        // Delete all related XrefServiceProgramVehicle entries

        // Delete the Service Program

        // Save changes
        await _serviceProgramRepository.SaveChangesAsync();

        // Return the deleted ServiceProgramID
        return request.ServiceProgramID;
    }
}