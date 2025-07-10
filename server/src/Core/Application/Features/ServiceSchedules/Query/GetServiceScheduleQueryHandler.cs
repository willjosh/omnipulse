using Application.Contracts.Logger;
using Application.Contracts.Persistence;
using Application.Exceptions;

using AutoMapper;

using Domain.Entities;

using MediatR;

namespace Application.Features.ServiceSchedules.Query.GetServiceSchedule;

public class GetServiceScheduleQueryHandler : IRequestHandler<GetServiceScheduleQuery, GetServiceScheduleDTO>
{
    private readonly IServiceScheduleRepository _serviceScheduleRepository;
    private readonly IAppLogger<GetServiceScheduleQueryHandler> _logger;
    private readonly IMapper _mapper;

    public GetServiceScheduleQueryHandler(
        IServiceScheduleRepository serviceScheduleRepository,
        IAppLogger<GetServiceScheduleQueryHandler> logger,
        IMapper mapper)
    {
        _serviceScheduleRepository = serviceScheduleRepository;
        _logger = logger;
        _mapper = mapper;
    }

    public async Task<GetServiceScheduleDTO> Handle(GetServiceScheduleQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation($"GetServiceScheduleDetailsQuery for ServiceScheduleID: {request.ServiceScheduleID}");
        var serviceSchedule = await _serviceScheduleRepository.GetByIdAsync(request.ServiceScheduleID);
        if (serviceSchedule == null)
        {
            _logger.LogError($"ServiceSchedule with ID {request.ServiceScheduleID} not found.");
            throw new EntityNotFoundException(typeof(ServiceSchedule).ToString(), "ServiceScheduleID", request.ServiceScheduleID.ToString());
        }
        var dto = _mapper.Map<GetServiceScheduleDTO>(serviceSchedule);
        _logger.LogInformation($"Returning ServiceScheduleDetailsDTO for ServiceScheduleID: {request.ServiceScheduleID}");
        return dto;
    }
}