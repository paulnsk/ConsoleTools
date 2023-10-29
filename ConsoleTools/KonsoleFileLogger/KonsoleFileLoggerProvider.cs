using Microsoft.Extensions.Logging;
using System.Collections.Generic;

namespace ConsoleTools.KonsoleFileLogger;

public class KonsoleFileLoggerProvider : ILoggerProvider
{
    private readonly string _filePath;
    private readonly List<string> _suppressedCategories;

    public KonsoleFileLoggerProvider(string filePath, List<string> suppressedCategories = default)
    {
        _filePath = filePath;
        _suppressedCategories = suppressedCategories;
    }

    public ILogger CreateLogger(string categoryName)
    {
        return new KonsoleFileLogger(categoryName, _filePath, _suppressedCategories);
    }

    public void Dispose()
    {
    }
}