namespace Application.Contracts.Logger;

/// <summary>
/// Defines a generic application logger for logging information, warnings, errors, debug, critical, and trace messages.
/// </summary>
/// <typeparam name="T">The type whose name is used for the logger category.</typeparam>
public interface IAppLogger<T>
{
    void LogInformation(string message);
    void LogInformation(string message, params object[] args);
    void LogInformation(Exception exception, string message);
    void LogInformation(Exception exception, string message, params object[] args);

    void LogWarning(string message);
    void LogWarning(string message, params object[] args);
    void LogWarning(Exception exception, string message);
    void LogWarning(Exception exception, string message, params object[] args);

    void LogError(string message);
    void LogError(string message, params object[] args);
    void LogError(Exception exception, string message);
    void LogError(Exception exception, string message, params object[] args);
    void LogError(Exception exception); // Just log the exception

    void LogDebug(string message);
    void LogDebug(string message, params object[] args);
    void LogDebug(Exception exception, string message);
    void LogDebug(Exception exception, string message, params object[] args);

    void LogCritical(string message);
    void LogCritical(string message, params object[] args);
    void LogCritical(Exception exception, string message);
    void LogCritical(Exception exception, string message, params object[] args);

    void LogTrace(string message);
    void LogTrace(string message, params object[] args);
}