using Microsoft.Extensions.Logging;

namespace ConsoleTools.KonsoleFileLogger;

public class KonsoleFileLoggerProvider : ILoggerProvider
{
    private readonly string _filePath;

    public KonsoleFileLoggerProvider(string filePath)
    {
        _filePath = filePath;
    }

    public ILogger CreateLogger(string categoryName)
    {
        return new KonsoleFileLogger(categoryName, _filePath);
    }

    public void Dispose()
    {
    }
}