using Application.Contracts.Logger;
using Application.Contracts.Persistence;
using Application.Exceptions;

using AutoMapper;

using Domain.Entities;

using MediatR;

namespace Application.Features.Users.Query.GetTechnician;

public class GetTechnicianQueryHandler : IRequestHandler<GetTechnicianQuery, GetTechnicianDTO>
{
    private readonly IUserRepository _userRepository;
    private readonly IAppLogger<GetTechnicianQueryHandler> _logger;
    private readonly IMapper _mapper;

    public GetTechnicianQueryHandler(IUserRepository userRepository, IAppLogger<GetTechnicianQueryHandler> logger, IMapper mapper)
    {
        _userRepository = userRepository;
        _logger = logger;
        _mapper = mapper;
    }

    public async Task<GetTechnicianDTO> Handle(GetTechnicianQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation($"GetTechnicianQuery for UserID: {request.Id}");

        // Retrieve technician by ID
        var technician = await _userRepository.GetTechnicianByIdAsync(request.Id);

        if (technician == null)
        {
            _logger.LogError($"Technician with ID {request.Id} not found.");
            throw new EntityNotFoundException(typeof(User).ToString(), "UserID", request.Id.ToString());
        }

        // Map entity to DTO
        var technicianDTO = _mapper.Map<GetTechnicianDTO>(technician);

        _logger.LogInformation($"Returning GetTechnicianDTO for UserID: {request.Id}");
        return technicianDTO;
    }
}