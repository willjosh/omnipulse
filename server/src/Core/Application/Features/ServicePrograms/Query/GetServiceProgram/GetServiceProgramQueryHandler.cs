using Application.Contracts.Logger;
using Application.Contracts.Persistence;
using Application.Exceptions;
using Application.Features.ServiceSchedules.Query;
using Application.Features.ServiceTasks.Query;

using AutoMapper;

using Domain.Entities;

using MediatR;

namespace Application.Features.ServicePrograms.Query.GetServiceProgram;

public sealed class GetServiceProgramQueryHandler : IRequestHandler<GetServiceProgramQuery, ServiceProgramDTO>
{
    private readonly IServiceProgramRepository _serviceProgramRepository;
    private readonly IServiceScheduleRepository _serviceScheduleRepository;
    private readonly IXrefServiceScheduleServiceTaskRepository _xrefServiceScheduleServiceTaskRepository;
    private readonly IServiceTaskRepository _serviceTaskRepository;
    private readonly IXrefServiceProgramVehicleRepository _xrefServiceProgramVehicleRepository;
    private readonly IAppLogger<GetServiceProgramQueryHandler> _logger;
    private readonly IMapper _mapper;

    public GetServiceProgramQueryHandler(
        IServiceProgramRepository serviceProgramRepository,
        IServiceScheduleRepository serviceScheduleRepository,
        IXrefServiceScheduleServiceTaskRepository xrefServiceScheduleServiceTaskRepository,
        IServiceTaskRepository serviceTaskRepository,
        IXrefServiceProgramVehicleRepository xrefServiceProgramVehicleRepository,
        IAppLogger<GetServiceProgramQueryHandler> logger,
        IMapper mapper)
    {
        _serviceProgramRepository = serviceProgramRepository;
        _serviceScheduleRepository = serviceScheduleRepository;
        _xrefServiceScheduleServiceTaskRepository = xrefServiceScheduleServiceTaskRepository;
        _serviceTaskRepository = serviceTaskRepository;
        _xrefServiceProgramVehicleRepository = xrefServiceProgramVehicleRepository;
        _logger = logger;
        _mapper = mapper;
    }

    public async Task<ServiceProgramDTO> Handle(GetServiceProgramQuery request, CancellationToken cancellationToken)
    {
        // Get Service Program by ID
        var serviceProgram = await _serviceProgramRepository.GetByIdAsync(request.ServiceProgramID);
        if (serviceProgram == null)
        {
            _logger.LogError($"{nameof(ServiceProgram)} with {nameof(ServiceProgram.ID)} {request.ServiceProgramID} not found.");
            throw new EntityNotFoundException(nameof(ServiceProgram), nameof(request.ServiceProgramID), request.ServiceProgramID.ToString());
        }

        // Get all Service Schedules in the Service Program
        var serviceSchedules = await _serviceScheduleRepository.GetAllByServiceProgramIDAsync(serviceProgram.ID);

        // Get all Xref records for all Service Schedules
        var serviceScheduleIds = serviceSchedules.Select(ss => ss.ID).ToList();
        var allXrefs = await _xrefServiceScheduleServiceTaskRepository.GetByServiceScheduleIdsAsync(serviceScheduleIds);

        // Get all unique Service Task IDs
        var serviceTaskIds = allXrefs.Select(xref => xref.ServiceTaskID).Distinct().ToList();

        // Get all Service Tasks in a single database call
        var serviceTasks = await _serviceTaskRepository.GetByIdsAsync(serviceTaskIds);
        if (serviceTasks == null)
        {
            _logger.LogError($"{nameof(_serviceTaskRepository.GetByIdsAsync)} returned null for {nameof(serviceTaskIds)}: {string.Join(", ", serviceTaskIds)}");
            throw new InvalidOperationException($"{nameof(_serviceTaskRepository.GetByIdsAsync)} returned null when non-null was expected");
        }

        // Create a lookup for quick access
        var serviceTaskLookup = serviceTasks.ToDictionary(st => st.ID);

        // Map ServiceSchedules and their ServiceTasks
        var serviceScheduleDTOs = serviceSchedules.Select(schedule =>
        {
            var scheduleDto = _mapper.Map<ServiceScheduleDTO>(schedule);
            // Get xrefs for this specific schedule
            var scheduleXrefs = allXrefs.Where(xref => xref.ServiceScheduleID == schedule.ID).ToList();

            // Map service tasks for this schedule
            scheduleDto.ServiceTasks = scheduleXrefs
                .Where(xref => serviceTaskLookup.ContainsKey(xref.ServiceTaskID))
                .Select(xref => _mapper.Map<ServiceTaskDTO>(serviceTaskLookup[xref.ServiceTaskID]))
                .ToList();

            return scheduleDto;
        }).ToList();

        // Extract assigned vehicle IDs from vehicle xref table
        var vehicleXrefs = await _xrefServiceProgramVehicleRepository.GetByServiceProgramIDAsync(serviceProgram.ID);
        var assignedVehicleIDs = vehicleXrefs.Select(x => x.VehicleID).ToList();

        // Compose ServiceProgramDTO
        var serviceProgramDTO = _mapper.Map<ServiceProgramDTO>(serviceProgram);
        serviceProgramDTO.ServiceSchedules = serviceScheduleDTOs;
        serviceProgramDTO.AssignedVehicleIDs = assignedVehicleIDs;

        return serviceProgramDTO;
    }
}