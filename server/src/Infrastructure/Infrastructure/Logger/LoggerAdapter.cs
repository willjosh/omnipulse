using Application.Contracts.Logger;

using Microsoft.Extensions.Logging;

namespace Infrastructure.Logger;

public class AppLogger<T> : IAppLogger<T>
{
    private readonly ILogger<T> _logger;

    public AppLogger(ILogger<T> logger)
    {
        _logger = logger;
    }

    public void LogInformation(string message)
        => _logger.LogInformation(message);

    public void LogInformation(string message, params object[] args)
        => _logger.LogInformation(message, args);

    public void LogInformation(Exception exception, string message)
        => _logger.LogInformation(exception, message);

    public void LogInformation(Exception exception, string message, params object[] args)
        => _logger.LogInformation(exception, message, args);

    public void LogWarning(string message)
        => _logger.LogWarning(message);

    public void LogWarning(string message, params object[] args)
        => _logger.LogWarning(message, args);

    public void LogWarning(Exception exception, string message)
        => _logger.LogWarning(exception, message);

    public void LogWarning(Exception exception, string message, params object[] args)
        => _logger.LogWarning(exception, message, args);

    public void LogError(string message)
        => _logger.LogError(message);

    public void LogError(string message, params object[] args)
        => _logger.LogError(message, args);

    public void LogError(Exception exception, string message)
        => _logger.LogError(exception, message);

    public void LogError(Exception exception, string message, params object[] args)
        => _logger.LogError(exception, message, args);

    public void LogError(Exception exception)
        => _logger.LogError(exception, exception.Message);

    public void LogDebug(string message)
        => _logger.LogDebug(message);

    public void LogDebug(string message, params object[] args)
        => _logger.LogDebug(message, args);

    public void LogDebug(Exception exception, string message)
        => _logger.LogDebug(exception, message);

    public void LogDebug(Exception exception, string message, params object[] args)
        => _logger.LogDebug(exception, message, args);

    public void LogCritical(string message)
        => _logger.LogCritical(message);

    public void LogCritical(string message, params object[] args)
        => _logger.LogCritical(message, args);

    public void LogCritical(Exception exception, string message)
        => _logger.LogCritical(exception, message);

    public void LogCritical(Exception exception, string message, params object[] args)
        => _logger.LogCritical(exception, message, args);

    public void LogTrace(string message)
        => _logger.LogTrace(message);

    public void LogTrace(string message, params object[] args)
        => _logger.LogTrace(message, args);
}