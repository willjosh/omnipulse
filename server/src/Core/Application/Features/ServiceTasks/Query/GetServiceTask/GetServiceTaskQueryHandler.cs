using Application.Contracts.Logger;
using Application.Contracts.Persistence;
using Application.Exceptions;

using AutoMapper;

using Domain.Entities;

using MediatR;

namespace Application.Features.ServiceTasks.Query.GetServiceTask;

/// <summary>
/// Handles <see cref="GetServiceTaskQuery"/> requests by retrieving the service task entity from the repository, mapping it to <see cref="ServiceTaskDTO"/>, and returning it.
/// </summary>
public class GetServiceTaskQueryHandler : IRequestHandler<GetServiceTaskQuery, ServiceTaskDTO>
{
    private readonly IServiceTaskRepository _serviceTaskRepository;
    private readonly IAppLogger<GetServiceTaskQueryHandler> _logger;
    private readonly IMapper _mapper;

    public GetServiceTaskQueryHandler(IServiceTaskRepository serviceTaskRepository, IAppLogger<GetServiceTaskQueryHandler> logger, IMapper mapper)
    {
        _serviceTaskRepository = serviceTaskRepository;
        _logger = logger;
        _mapper = mapper;
    }

    public async Task<ServiceTaskDTO> Handle(GetServiceTaskQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation($"GetServiceTaskQuery for ServiceTaskID: {request.ServiceTaskID}");

        // Retrieve service task by ID
        var serviceTask = await _serviceTaskRepository.GetByIdAsync(request.ServiceTaskID);

        if (serviceTask == null)
        {
            _logger.LogError($"ServiceTask with ID {request.ServiceTaskID} not found.");
            throw new EntityNotFoundException(typeof(ServiceTask).ToString(), "ServiceTaskID", request.ServiceTaskID.ToString());
        }

        // Map entity to DTO
        var dto = _mapper.Map<ServiceTaskDTO>(serviceTask);

        _logger.LogInformation($"Returning GetServiceTaskDTO for ServiceTaskID: {request.ServiceTaskID}");
        return dto;
    }
}