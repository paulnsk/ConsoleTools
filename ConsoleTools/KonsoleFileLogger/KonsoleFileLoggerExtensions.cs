using System;
using System.IO;
using Microsoft.Extensions.Logging;

namespace ConsoleTools.KonsoleFileLogger;

public static class KonsoleFileLoggerExtensions
{
    public static ILoggingBuilder AddKonsoleFile(this ILoggingBuilder builder, string filePath = null, bool deleteOldLog = false)
    {
        filePath ??= U.ExePath() + ".k.log";
        if (deleteOldLog && File.Exists(filePath)) File.Delete(filePath);
        var dir = Path.GetDirectoryName(filePath);
        if (!Directory.Exists(dir)) Directory.CreateDirectory(dir!);
        if (!Directory.Exists(dir)) throw new Exception($"We tried creating {dir} but failed");
        builder.AddProvider(new KonsoleFileLoggerProvider(filePath));
        return builder;
    }
}