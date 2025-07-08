using System;
using System.Data;

using Domain.Entities.Enums;

using FluentValidation;

namespace Application.Features.WorkOrderLineItem.Models;

public class CreateWorkOrderLineItemDTOValidator : AbstractValidator<CreateWorkOrderLineItemDTO>
{
    public CreateWorkOrderLineItemDTOValidator()
    {
        // BASIC REQUIRED FIELDS VALIDATION

        RuleFor(x => x.WorkOrderID)
            .GreaterThan(0)
            .WithMessage("Work Order ID must be greater than 0");

        RuleFor(x => x.ServiceTaskID)
            .GreaterThan(0)
            .WithMessage("Service Task ID must be greater than 0");

        RuleFor(x => x.ItemType)
            .IsInEnum()
            .WithMessage("Valid item type is required");

        RuleFor(x => x.Quantity)
            .GreaterThan(0)
            .WithMessage("Quantity must be greater than 0");

        // OPTIONAL FIELDS VALIDATION
        RuleFor(x => x.Description)
            .MaximumLength(500)
            .WithMessage("Description cannot exceed 500 characters")
            .When(x => !string.IsNullOrEmpty(x.Description));

        RuleFor(x => x.AssignedToUserID)
            .NotEmpty()
            .WithMessage("Assigned User ID cannot be empty")
            .When(x => !string.IsNullOrEmpty(x.AssignedToUserID));

        RuleFor(x => x.InventoryItemID)
            .GreaterThan(0)
            .WithMessage("Inventory Item ID must be greater than 0")
            .When(x => x.InventoryItemID.HasValue);

        // PRICING FIELDS VALIDATION
        RuleFor(x => x.UnitPrice)
            .GreaterThan(0)
            .WithMessage("Unit Price must be greater than 0")
            .When(x => x.UnitPrice.HasValue);

        RuleFor(x => x.HourlyRate)
            .GreaterThan(0)
            .WithMessage("Hourly Rate must be greater than 0")
            .When(x => x.HourlyRate.HasValue);

        RuleFor(x => x.LaborHours)
            .GreaterThan(0)
            .LessThanOrEqualTo(24)
            .WithMessage("Labor Hours must be between 0 and 24")
            .When(x => x.LaborHours.HasValue);

        // PARTS-ONLY VALIDATION
        When(x => x.ItemType == LineItemTypeEnum.PARTS, () =>
        {
            RuleFor(x => x.InventoryItemID)
                .NotNull()
                .GreaterThan(0)
                .WithMessage("Inventory Item ID is required when item type is PARTS");

            RuleFor(x => x.UnitPrice)
                .NotNull()
                .GreaterThan(0)
                .WithMessage("Unit Price is required and must be greater than 0 when item type is PARTS");

            RuleFor(x => x.AssignedToUserID)
                .NotEmpty()
                .WithMessage("Assigned User ID is required for parts");

            // Ensure labor fields are NOT provided for parts-only
            RuleFor(x => x.HourlyRate)
                .Null()
                .WithMessage("Hourly rate should not be specified for PARTS-only items");

            RuleFor(x => x.LaborHours)
                .Null()
                .WithMessage("Labor hours should not be specified for PARTS-only items");
        });

        // ITEM-ONLY VALIDATION (Labor/Service)
        When(x => x.ItemType == LineItemTypeEnum.ITEM, () =>
        {
            RuleFor(x => x.HourlyRate)
                .NotNull()
                .GreaterThan(0)
                .WithMessage("Hourly rate is required and must be greater than 0 when item type is ITEM");

            RuleFor(x => x.LaborHours)
                .NotNull()
                .GreaterThan(0)
                .WithMessage("Labor hours are required and must be greater than 0 when item type is ITEM");

            RuleFor(x => x.AssignedToUserID)
                .NotEmpty()
                .WithMessage("Assigned User ID is required for labor items");

            // For labor items, quantity should typically be 1
            RuleFor(x => x.Quantity)
                .Equal(1)
                .WithMessage("Quantity should be 1 for ITEM-only (labor) items - use Labor Hours for duration");

            // Ensure parts fields are NOT provided for item-only
            RuleFor(x => x.InventoryItemID)
                .Null()
                .WithMessage("Inventory Item ID should not be specified for ITEM-only (labor) items");

            RuleFor(x => x.UnitPrice)
                .Null()
                .WithMessage("Unit Price should not be specified for ITEM-only (labor) items");
        });

        // BOTH VALIDATION - Both parts AND labor must be present
        When(x => x.ItemType == LineItemTypeEnum.BOTH, () =>
        {
            // PARTS requirements for BOTH
            RuleFor(x => x.InventoryItemID)
                .NotNull()
                .GreaterThan(0)
                .WithMessage("Inventory Item ID is required when item type is BOTH");

            RuleFor(x => x.UnitPrice)
                .NotNull()
                .GreaterThan(0)
                .WithMessage("Unit Price is required when item type is BOTH");

            // LABOR requirements for BOTH
            RuleFor(x => x.HourlyRate)
                .NotNull()
                .GreaterThan(0)
                .WithMessage("Hourly rate is required when item type is BOTH");

            RuleFor(x => x.LaborHours)
                .NotNull()
                .GreaterThan(0)
                .WithMessage("Labor hours are required when item type is BOTH");

            RuleFor(x => x.AssignedToUserID)
                .NotEmpty()
                .WithMessage("Assigned User ID is required when item type is BOTH");
        });

        // CROSS-VALIDATION - Ensure valid field combinations
        RuleFor(x => x)
            .Must(HaveValidFieldCombination)
            .WithMessage("Invalid field combination for the specified item type");
    }

    // Helper method to validate field combinations
    private static bool HaveValidFieldCombination(CreateWorkOrderLineItemDTO item)
    {
        return item.ItemType switch
        {
            LineItemTypeEnum.PARTS =>
                item.InventoryItemID.HasValue &&
                item.UnitPrice.HasValue &&
                !item.HourlyRate.HasValue &&
                !item.LaborHours.HasValue,

            LineItemTypeEnum.ITEM =>
                item.HourlyRate.HasValue &&
                item.LaborHours.HasValue &&
                !item.InventoryItemID.HasValue &&
                !item.UnitPrice.HasValue &&
                item.Quantity == 1,

            LineItemTypeEnum.BOTH =>
                item.InventoryItemID.HasValue &&
                item.UnitPrice.HasValue &&
                item.HourlyRate.HasValue &&
                item.LaborHours.HasValue,

            _ => false
        };
    }
}