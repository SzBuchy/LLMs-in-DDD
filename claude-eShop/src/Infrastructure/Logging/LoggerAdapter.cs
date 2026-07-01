using ApplicationCore.Interfaces;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Logging;

public class LoggerAdapter<T> : IAppLogger<T>
{
    private readonly ILogger<T> _logger;

    public LoggerAdapter(ILogger<T> logger)
    {
        _logger = logger;
    }

    public void LogInformation(string message, params object[] args) => _logger.LogInformation(message, args);

    public void LogWarning(string message, params object[] args) => _logger.LogWarning(message, args);

    public void LogError(Exception? exception, string message, params object[] args) => _logger.LogError(exception, message, args);
}
