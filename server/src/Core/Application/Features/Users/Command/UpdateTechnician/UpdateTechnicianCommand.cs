using System;

using MediatR;

namespace Application.Features.Users.Command.UpdateTechnician;

public record UpdateTechnicianCommand(
    string Id,
    string? FirstName,
    string? LastName,
    DateTime? HireDate,
    bool? IsActive 
) : IRequest<string>
{ }