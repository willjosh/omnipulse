using System;

namespace Application.Features.Users.Query.GetTechnician;

public class GetTechnicianDTO
{ 
    public required string Id { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public required DateTime HireDate { get; set; }
    public required bool IsActive { get; set; } = true;
    public required string Email { get; set; }
}
