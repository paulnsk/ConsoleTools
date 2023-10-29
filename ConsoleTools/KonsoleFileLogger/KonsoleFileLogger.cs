using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Extensions.Logging;

namespace ConsoleTools.KonsoleFileLogger
{

    /// <summary>
    /// An implementation of ILogger which writes simultaneously to console (with color syntax sypport by Konsole) and to file. Use ClearProviders() and then AddKonsoleFile() extension to inject the provider.
    /// </summary>
    public class KonsoleFileLogger : ILogger
    {
        private readonly string _categoryName;
        private readonly string _filePath;

        public List<string> SuppressedCategories { get; set; } = new();

        public KonsoleFileLogger(string categoryName, string filePath, List<string> suppressedCategories = default)
        {
            _categoryName = categoryName;
            _filePath = filePath;
            SuppressedCategories = suppressedCategories;
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

            if (SuppressedCategories?.Contains(_categoryName) == true) return;

            var message = formatter(state, exception);
            if (string.IsNullOrEmpty(message))
            {
                return;
            }

            message = $"♦c{DateTime.Now:yyyy.dd.MM HH:mm:ss:fff} ♦y[{ColoredLogLevel(logLevel)}♦y] ♦Y{_categoryName}♦y:♦w {message}";

            lock (WriteLock)
            {
                Konsole.WriteLine(message);
                File.AppendAllText(_filePath, Konsole.UnEscape(message) + Environment.NewLine);
            }

        }

        public bool IsEnabled(Microsoft.Extensions.Logging.LogLevel logLevel)
        {
            return true;
        }

        public IDisposable? BeginScope<TState>(TState state) where TState : notnull => default;

    }
}
