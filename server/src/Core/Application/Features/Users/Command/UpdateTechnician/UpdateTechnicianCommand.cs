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
{
    public bool ShouldUpdateFirstName => FirstName is not null;
    public bool ShouldUpdateLastName => LastName is not null;
    public bool ShouldUpdateHireDate => HireDate is not null;
    public bool ShouldUpdateIsActive => IsActive is not null;

    public bool ShouldUpdate =>
        ShouldUpdateFirstName || ShouldUpdateLastName || ShouldUpdateHireDate || ShouldUpdateIsActive;
}