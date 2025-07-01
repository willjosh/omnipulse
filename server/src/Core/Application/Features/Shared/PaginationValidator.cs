using System;
using Application.Models;
using FluentValidation;

namespace Application.Features.Shared;

public class PaginationValidator<T> : AbstractValidator<PaginationParameters>
{
    public PaginationValidator(string[] validSortFields)
    {
        // All your common validation rules
        RuleFor(x => x.PageNumber)
            .GreaterThan(0)
            .WithMessage("PageNumber must be greater than 0.");

        RuleFor(x => x.PageSize)
            .GreaterThan(0)
            .WithMessage("PageSize must be greater than 0.");

        RuleFor(x => x.Search)
            .MaximumLength(100)
            .WithMessage("Search must not exceed 100 characters.");
        
        // Custom sort fields per entity
        RuleFor(x => x.SortBy)
            .Must(sortBy => IsValidSortBy(sortBy, validSortFields))
            .WithMessage($"SortBy must be one of: {string.Join(", ", validSortFields)}");
    }
    
    private static bool IsValidSortBy(string? sortBy, string[] ValidSortFields)
    {
        if (sortBy == null) {
            return true; 
        }

        return ValidSortFields.Contains(sortBy.ToLowerInvariant());
    }
}
