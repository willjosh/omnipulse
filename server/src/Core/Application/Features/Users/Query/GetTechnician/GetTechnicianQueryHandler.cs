using System;

using Application.Contracts.Logger;
using Application.Contracts.Persistence;

using AutoMapper;

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

    public Task<GetTechnicianDTO> Handle(GetTechnicianQuery request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
