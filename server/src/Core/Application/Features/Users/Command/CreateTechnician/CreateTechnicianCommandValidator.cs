using System;
using FluentValidation;

namespace Application.Features.Users.Command.CreateTechnician;

public class CreateTechnicianCommandValidator: AbstractValidator<CreateTechnicianCommand>
{
    public CreateTechnicianCommandValidator()
    {
        
    }
}
