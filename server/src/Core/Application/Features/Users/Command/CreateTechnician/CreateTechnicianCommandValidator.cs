using System;
using FluentValidation;

namespace Application.Features.Users.Command.CreateTechnician;

public class CreateTechnicianCommandValidator : AbstractValidator<CreateTechnicianCommand>
{
    public CreateTechnicianCommandValidator()
    {
        // Email validation
        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("Email is required")
            .EmailAddress()
            .WithMessage("Email must be a valid email address")
            .MaximumLength(256)
            .WithMessage("Email must not exceed 256 characters");

        // Password validation
        RuleFor(x => x.Password)
            .NotEmpty()
            .WithMessage("Password is required")
            .MinimumLength(6)
            .WithMessage("Password must be at least 6 characters long")
            .MaximumLength(128)
            .WithMessage("Password must not exceed 128 characters")
            .Must(password => !string.IsNullOrWhiteSpace(password) && password.Trim() == password)
            .WithMessage("Password cannot contain leading or trailing whitespace")
            .Must(password => !password.Contains(" ") && !password.Contains("\t") && !password.Contains("\n"))
            .WithMessage("Password cannot contain whitespace characters")
            .Must(HaveUppercaseLetter)
            .WithMessage("Password must contain at least one uppercase letter")
            .Must(HaveLowercaseLetter)
            .WithMessage("Password must contain at least one lowercase letter")
            .Must(HaveDigit)
            .WithMessage("Password must contain at least one number")
            .Must(HaveSpecialCharacter)
            .WithMessage("Password must contain at least one special character")
            .Must(NotBeCommonPassword)
            .WithMessage("Password is too common and not secure");

        // FirstName validation
        RuleFor(x => x.FirstName)
            .NotEmpty()
            .WithMessage("First name is required")
            .MaximumLength(100)
            .WithMessage("First name must not exceed 100 characters");

        // LastName validation
        RuleFor(x => x.LastName)
            .NotEmpty()
            .WithMessage("Last name is required")
            .MaximumLength(100)
            .WithMessage("Last name must not exceed 100 characters");

        // HireDate validation
        RuleFor(x => x.HireDate)
            .Must(BeValidHireDate)
            .WithMessage("Hire date must be between 1900-01-01 and today's date");
    }

    private static bool HaveUppercaseLetter(string password)
    {
        return !string.IsNullOrEmpty(password) && password.Any(char.IsUpper);
    }

    private static bool HaveLowercaseLetter(string password)
    {
        return !string.IsNullOrEmpty(password) && password.Any(char.IsLower);
    }

    private static bool HaveDigit(string password)
    {
        return !string.IsNullOrEmpty(password) && password.Any(char.IsDigit);
    }

    private static bool HaveSpecialCharacter(string password)
    {
        var specialCharacters = "!@#$%^&*()_+-=[]{}|;:,.<>?";
        return !string.IsNullOrEmpty(password) && password.Any(specialCharacters.Contains);
    }

    private static bool NotBeCommonPassword(string password)
    {
        if (string.IsNullOrEmpty(password))
            return false;

        var commonPasswords = new[]
        {
            "password", "123456", "123456789", "qwerty", "abc123",
            "password123", "admin", "letmein", "welcome", "monkey",
            "1234567890", "qwerty123", "password1", "123123"
        };

        return !commonPasswords.Contains(password.ToLowerInvariant());
    }

    private static bool BeValidHireDate(DateTime hireDate)
    {
        var minDate = new DateTime(1900, 1, 1);
        var maxDate = DateTime.UtcNow.Date;

        return hireDate.Date >= minDate && hireDate.Date <= maxDate;
    }
}