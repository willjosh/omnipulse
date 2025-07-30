namespace Application.Features.Users.Query.GetAllTechnician;

public class GetAllTechnicianDTO
{
    /// <example>566ae2d4-a781-4690-84c0-f8b284868e43</example>
    public required string Id { get; set; }

    /// <example>John</example>
    public required string FirstName { get; set; }

    /// <example>Smith</example>
    public required string LastName { get; set; }

    public required DateTime HireDate { get; set; }

    /// <example>true</example>
    public required bool IsActive { get; set; } = true;

    /// <example>john.smith@omnipulse.com</example>
    public required string Email { get; set; }
}