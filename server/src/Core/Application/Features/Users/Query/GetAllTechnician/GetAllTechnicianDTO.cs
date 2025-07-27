namespace Application.Features.Users.Query.GetAllTechnician;

public class GetAllTechnicianDTO
{
    public required string Id { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public required DateTime HireDate { get; set; }
    public required bool IsActive { get; set; } = true;
    public required string Email { get; set; }
}