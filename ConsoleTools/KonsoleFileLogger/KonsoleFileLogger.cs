using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace ConsoleTools.KonsoleFileLogger;

//todo TEST ME and clean me up:

/*
 *
 
    using (_logger.BeginScope(new Dictionary<string, object> { ["EventId"] = 123 }))
   {
       _logger.LogInformation("Processing event...");
   }
 
 *
 */

/// <summary>
/// An implementation of ILogger which writes simultaneously to console (with color syntax sypport by Konsole) and to file. Use ClearProviders() and then AddKonsoleFile() extension to inject the provider.
/// </summary>
public class KonsoleFileLogger : ILogger
{
    private readonly string _categoryName;
    private readonly string _filePath;
    private readonly KonsoleFileLoggerConfig _config;

    // --- ДОБАВЛЕНО: Потокобезопасное хранилище для Scopes ---
    private static readonly AsyncLocal<Stack<object>> _scopes = new();

    public KonsoleFileLogger(string categoryName, string filePath, KonsoleFileLoggerConfig config)
    {
        _categoryName = categoryName;
        _filePath = filePath;
        _config = config;
    }


    private string ColoredLogLevel(Microsoft.Extensions.Logging.LogLevel logLevel)
    {
        switch (logLevel)
        {
            case Microsoft.Extensions.Logging.LogLevel.Trace:
                return $"♦m{logLevel}";
            case Microsoft.Extensions.Logging.LogLevel.Debug:
                return $"♦g{logLevel}";
            case Microsoft.Extensions.Logging.LogLevel.Information:
                return $"♦b{logLevel}";
            case Microsoft.Extensions.Logging.LogLevel.Warning:
                return $"♦y{logLevel}";
            case Microsoft.Extensions.Logging.LogLevel.Error:
                return $"♦r{logLevel}";
            case Microsoft.Extensions.Logging.LogLevel.Critical:
                return $"♦R{logLevel}";
            case Microsoft.Extensions.Logging.LogLevel.None:
                return $"♦w{logLevel}";
            default:
                throw new ArgumentOutOfRangeException(nameof(logLevel), logLevel, null);
        }
    }

    private static readonly object WriteLock = new object();

    public void Log<TState>(Microsoft.Extensions.Logging.LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        if (!IsEnabled(logLevel))
        {
            return;
        }

        if (_config.SuppressedCategories?.Contains(_categoryName) == true) return;

        var message = formatter(state, exception);
        if (string.IsNullOrEmpty(message) && exception == null) // --- ИЗМЕНЕНО: Проверяем и exception ---
        {
            return;
        }

        // --- ДОБАВЛЕНО: Форматируем scopes ---
        var scopeString = FormatScopes();

        // --- ИЗМЕНЕНО: Добавляем scopeString в сообщение. Также добавляем exception, если он есть ---
        message = $"♦c{DateTime.Now:yyyy.dd.MM HH:mm:ss:fff} ♦y[{ColoredLogLevel(logLevel)}♦y] ♦Y{_categoryName}♦w{scopeString}♦y:♦w {message}";
        if (exception != null)
        {
            message += Environment.NewLine + $"♦R{exception}";
        }


        lock (WriteLock)
        {
            Konsole.WriteLine(message);
            if (!_config.DisableFile) File.AppendAllText(_filePath, Konsole.UnEscape(message) + Environment.NewLine);
        }

    }
        
    private string FormatScopes()
    {
        var stack = _scopes.Value;
        if (stack == null || stack.Count == 0)
        {
            return string.Empty;
        }

        var sb = new StringBuilder();
        // Идем от внешнего scope к внутреннему
        foreach (var scope in stack.Reverse())
        {
            // Стандартный scope - это словарь
            if (scope is IEnumerable<KeyValuePair<string, object>> kvps)
            {
                foreach (var kvp in kvps)
                {
                    sb.Append($" ♦A{kvp.Key}♦w=♦m{kvp.Value}♦w");
                }
            }
            else // Или просто строка
            {
                sb.Append($" ♦g=>♦w {scope}");
            }
        }
        return sb.ToString();
    }


    public bool IsEnabled(Microsoft.Extensions.Logging.LogLevel logLevel)
    {
        return true;
    }
        
    public IDisposable? BeginScope<TState>(TState state) where TState : notnull
    {
        var stack = _scopes.Value ??= new Stack<object>();
        stack.Push(state);
        return new ScopePopper();
    }
        
    private sealed class ScopePopper : IDisposable
    {
        public void Dispose()
        {
            _scopes.Value?.Pop();
        }
    }
}