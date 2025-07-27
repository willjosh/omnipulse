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
}