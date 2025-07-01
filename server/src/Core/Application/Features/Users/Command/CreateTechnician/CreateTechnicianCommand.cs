using MediatR;

namespace Application.Features.Users.Command.CreateTechnician;

public record CreateTechnicianCommand : IRequest<Guid> { }