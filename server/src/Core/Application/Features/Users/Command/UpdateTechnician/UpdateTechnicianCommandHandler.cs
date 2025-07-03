using System;

using Application.Contracts.Logger;
using Application.Contracts.Persistence;

using AutoMapper;

using FluentValidation;

using MediatR;

namespace Application.Features.Users.Command.UpdateTechnician;

public class UpdateTechnicianCommandHandler : IRequestHandler<UpdateTechnicianCommand, string>
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;
    private readonly IAppLogger<UpdateTechnicianCommandHandler> _logger;
    private readonly IValidator<UpdateTechnicianCommand> _validator;

    public UpdateTechnicianCommandHandler(
        IUserRepository userRepository,
        IMapper mapper,
        IAppLogger<UpdateTechnicianCommandHandler> logger,
        IValidator<UpdateTechnicianCommand> validator)
    {
        _userRepository = userRepository;
        _mapper = mapper;
        _logger = logger;
        _validator = validator;    
    }

    public Task<string> Handle(UpdateTechnicianCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
