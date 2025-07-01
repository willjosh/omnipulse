using MediatR;

namespace Application.Features.Users.Command.CreateTechnician;

public record CreateTechnicianCommand(
   string FirstName,
   string LastName,
   DateTime HireDate,
   bool IsActive = true
) : IRequest<Guid>
{ }