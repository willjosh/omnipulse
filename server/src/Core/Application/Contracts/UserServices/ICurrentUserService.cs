using Domain.Entities;

namespace Application.Contracts.UserServices;

public interface ICurrentUserService
{
    string? UserId { get; }
    string? UserName { get; }
    string? Email { get; }
    bool IsAuthenticated { get; }
    bool IsInRole(string role);
    IEnumerable<string> Roles { get; }
    string? GetClaimValue(string claimType);
    IEnumerable<string> GetClaimValues(string claimType);
}