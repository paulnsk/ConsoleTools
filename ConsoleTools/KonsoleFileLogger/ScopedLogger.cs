using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

namespace ConsoleTools.KonsoleFileLogger;


//todo ТЕСТАНИ ЖЕ МЕНЯ!!
/*
 *
 *
 
   // 1. Создаем ОБЫЧНЫЙ логгер через фабрику
   var baseLogger = _loggerFactory.CreateLogger<TicketmasterIntegrationService>();
   
   // 2. Создаем словарь с контекстом, который будет жить ВЕЧНО с этим логгером
   var permanentContext = new Dictionary<string, object> { ["Event"] = evt.Name };

   // 3. Создаем "обертку", которая "приклеивает" контекст к базовому логгеру
   var svcLogger = new ScopedLogger(baseLogger, permanentContext);

   // 4. Передаем этот новый, обогащенный логгер в сервис
   svc = new TicketmasterIntegrationService(svcLogger); 


 *
 */
public class ScopedLogger : ILogger
{
    private readonly ILogger _underlyingLogger;
    private readonly Dictionary<string, object> _scopeValues;

    public ScopedLogger(ILogger underlyingLogger, Dictionary<string, object> scopeValues)
    {
        _underlyingLogger = underlyingLogger;
        _scopeValues = scopeValues;
    }

    public void Log<TState>(Microsoft.Extensions.Logging.LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        // При каждом вызове Log мы оборачиваем его в наш "вшитый" scope
        using (_underlyingLogger.BeginScope(_scopeValues))
        {
            _underlyingLogger.Log(logLevel, eventId, state, exception, formatter);
        }
    }

    public bool IsEnabled(Microsoft.Extensions.Logging.LogLevel logLevel) => _underlyingLogger.IsEnabled(logLevel);

    public IDisposable BeginScope<TState>(TState state) where TState : notnull => _underlyingLogger.BeginScope(state);
}