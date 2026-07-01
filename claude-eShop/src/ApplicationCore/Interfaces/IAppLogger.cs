namespace ApplicationCore.Interfaces;

// Keeps ApplicationCore free of a direct dependency on Microsoft.Extensions.Logging;
// Infrastructure supplies the real implementation backed by ILogger<T>.
public interface IAppLogger<T>
{
    void LogInformation(string message, params object[] args);
    void LogWarning(string message, params object[] args);
    void LogError(Exception? exception, string message, params object[] args);
}
