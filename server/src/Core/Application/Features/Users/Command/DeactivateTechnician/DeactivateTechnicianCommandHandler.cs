using System;

using MediatR;

namespace Application.Features.Users.Command.DeactivateTechnician;

public class DeactivateTechnicianCommandHandler : IRequestHandler<DeactivateTechnicianCommand, Guid>
{
    public Task<Guid> Handle(DeactivateTechnicianCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
