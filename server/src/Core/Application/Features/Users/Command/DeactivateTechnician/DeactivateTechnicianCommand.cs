using System;

using MediatR;

namespace Application.Features.Users.Command.DeactivateTechnician;

public record DeactivateTechnicianCommand(string Id) : IRequest<string> { }