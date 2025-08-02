using System;

using FluentValidation;

namespace Application.Features.Inventory.Command.CreateInventory;

public class CreateInventoryCommandValidator : AbstractValidator<CreateInventoryCommand>
{
    public CreateInventoryCommandValidator()
    {
    }
}