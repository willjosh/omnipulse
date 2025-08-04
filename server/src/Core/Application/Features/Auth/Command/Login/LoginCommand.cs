using Application.Contracts.AuthService;

using MediatR;

namespace Application.Features.Auth.Command;

public record LoginCommand(
    string Email,
    string Password
) : IRequest<AuthUserDTO>
{ }