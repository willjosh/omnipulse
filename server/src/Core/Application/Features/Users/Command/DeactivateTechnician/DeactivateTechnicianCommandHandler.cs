using System;

using Application.Contracts.Logger;
using Application.Contracts.Persistence;

using AutoMapper;

using MediatR;

namespace Application.Features.Users.Command.DeactivateTechnician;

public class DeactivateTechnicianCommandHandler : IRequestHandler<DeactivateTechnicianCommand, string>
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;
    private readonly IAppLogger<DeactivateTechnicianCommandHandler> _logger;

    public DeactivateTechnicianCommandHandler(IUserRepository userRepository, IMapper mapper, IAppLogger<DeactivateTechnicianCommandHandler> logger)
    {
        _userRepository = userRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public Task<string> Handle(DeactivateTechnicianCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}