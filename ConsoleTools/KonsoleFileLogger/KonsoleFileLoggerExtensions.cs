using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;
using Microsoft.Extensions.Logging;

namespace ConsoleTools.KonsoleFileLogger
{
    public static class KonsoleFileLoggerExtensions
    {

        /// <summary>
        /// Adds caller class and method names to the log message. Does not work in async context.
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="message"></param>
        /// <param name="callerName"></param>
        public static void LogDebugX(this ILogger logger, string message, [CallerMemberName] string callerName = "")
        {
            logger.LogDebug(GetformattedMessage(message, callerName));
        }

        /// <summary>
        /// Adds caller class and method names to the log message. Does not work in async context.
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="message"></param>
        /// <param name="callerName"></param>
        public static void LogInformationX(this ILogger logger, string message, [CallerMemberName] string callerName = "")
        {
            logger.LogInformation(GetformattedMessage(message, callerName));
        }

        /// <summary>
        /// Adds caller class and method names to the log message. Does not work in async context.
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="message"></param>
        /// <param name="callerName"></param>
        public static void LogCriticalX(this ILogger logger, string message, [CallerMemberName] string callerName = "")
        {
            logger.LogCritical(GetformattedMessage(message, callerName));
        }

        /// <summary>
        /// Adds caller class and method names to the log message. Does not work in async context.
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="message"></param>
        /// <param name="callerName"></param>
        public static void LogWarningX(this ILogger logger, string message, [CallerMemberName] string callerName = "")
        {
            logger.LogWarning(GetformattedMessage(message, callerName));
        }


        /// <summary>
        /// Adds caller class and method names to the log message. Does not work in async context.
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="message"></param>
        /// <param name="callerName"></param>
        public static void LogTraceX(this ILogger logger, string message, [CallerMemberName] string callerName = "")
        {
            logger.LogTrace(GetformattedMessage(message, callerName));
        }


        /// <summary>
        /// Adds caller class and method names to the log message. Does not work in async context.
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="message"></param>
        /// <param name="callerName"></param>
        public static void LogErrorX(this ILogger logger, string message, [CallerMemberName] string callerName = "")
        {
            logger.LogError(GetformattedMessage(message, callerName));
        }

        private static string GetformattedMessage(string message, string methodName)
        {
            var stackTrace = new StackTrace();
            var callerFrame = stackTrace.GetFrame(2);

            var className = callerFrame?.GetMethod()?.DeclaringType?.Name;
            if (!string.IsNullOrWhiteSpace(className)) className += "♦r.♦c";
            return $"♦c{className}{methodName}♦r:♦w {message}";
        }
    }
}
