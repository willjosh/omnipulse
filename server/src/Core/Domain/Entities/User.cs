using System;
using Microsoft.AspNetCore.Identity;

namespace Domain.Entities;

public class User : IdentityUser
{
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public required DateTime HireDate { get; set; }
    public required Boolean IsActive { get; set; } = true;
    public required DateTime CreatedAt { get; set; }
    public required DateTime UpdatedAt { get; set; }

}

