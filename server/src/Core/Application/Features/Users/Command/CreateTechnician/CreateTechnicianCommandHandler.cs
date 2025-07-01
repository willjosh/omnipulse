using System;
using Application.Contracts.Logger;
using Application.Contracts.Persistence;
using AutoMapper;
using FluentValidation;
using MediatR;

namespace Application.Features.Users.Command.CreateTechnician;

public class CreateTechnicianCommandHandler : IRequestHandler<CreateTechnicianCommand, Guid>
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;
    private readonly IAppLogger<CreateTechnicianCommandHandler> _logger;
    private readonly IValidator<CreateTechnicianCommand> _validator;

    public CreateTechnicianCommandHandler(IUserRepository userRepository, IMapper mapper, IAppLogger<CreateTechnicianCommandHandler> logger, IValidator<CreateTechnicianCommand> validator)
    {
        _userRepository = userRepository;
        _mapper = mapper;
        _logger = logger;
        _validator = validator;
    }

    public Task<Guid> Handle(CreateTechnicianCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
