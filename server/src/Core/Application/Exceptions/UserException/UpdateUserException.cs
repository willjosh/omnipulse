using System;

namespace Application.Exceptions.UserException;

public class UpdateUserException : Exception
{
    public string TechnicianId { get; }
    public IEnumerable<string> Errors { get; }

    public UpdateUserException(string technicianId, IEnumerable<string> errors)
        : base($"Failed to update technician with ID '{technicianId}': {string.Join(", ", errors)}")
    {
        TechnicianId = technicianId;
        Errors = errors;
    }

    public UpdateUserException(string technicianId, IEnumerable<string> errors, Exception innerException)
        : base($"Failed to update technician with ID '{technicianId}': {string.Join(", ", errors)}", innerException)
    {
        TechnicianId = technicianId;
        Errors = errors;
    }
}