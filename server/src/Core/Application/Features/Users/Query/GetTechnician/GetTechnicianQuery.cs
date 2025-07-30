using MediatR;

namespace Application.Features.Users.Query.GetTechnician;

public record GetTechnicianQuery(string Id) : IRequest<GetTechnicianDTO> { }