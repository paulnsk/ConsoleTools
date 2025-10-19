using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

namespace ConsoleTools.KonsoleFileLogger;

public class ScopedLogger(ILogger underlyingLogger, Dictionary<string, object> scopeValues) : ILogger
{
    public void Log<TState>(Microsoft.Extensions.Logging.LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        using (underlyingLogger.BeginScope(scopeValues))
        {
            underlyingLogger.Log(logLevel, eventId, state, exception, formatter);
        }
    }

    public bool IsEnabled(Microsoft.Extensions.Logging.LogLevel logLevel) => underlyingLogger.IsEnabled(logLevel);

    public IDisposable BeginScope<TState>(TState state) where TState : notnull => underlyingLogger.BeginScope(state);
}