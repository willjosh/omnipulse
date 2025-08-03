using System;

using MediatR;

namespace Application.Features.Auth.Command.Register;

public record RegisterOperatorCommand(
    string Email,
   string Password,
   string FirstName,
   string LastName,
   DateTime HireDate,
   bool IsActive = true
) : IRequest<Guid>
{
}