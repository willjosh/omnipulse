using System;

using Application.Contracts.Logger;
using Application.Contracts.Persistence;
using Application.Exceptions;
using Application.Exceptions.UserException;

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

    public async Task<string> Handle(DeactivateTechnicianCommand request, CancellationToken cancellationToken)
    {
        // check if the technician exists
        var user = await _userRepository.GetByIdAsync(request.Id);

        if (user == null)
        {
            _logger.LogError($"Technician with ID {request.Id} not found.");
            throw new EntityNotFoundException("Technician", "Id", request.Id);
        }

        // deactivate the technician
        user.IsActive = false;

        // update the technician in the repository
        var result = await _userRepository.UpdateAsync(user);
        if (result.Succeeded == false)
        {
            _logger.LogError($"Failed to deactivate technician with ID {request.Id}: {string.Join(", ", result.Errors.Select(e => e.Description))}");
            throw new UpdateUserException(request.Id, result.Errors.Select(e => e.Description));
        }

        return user.Id;
    }
}