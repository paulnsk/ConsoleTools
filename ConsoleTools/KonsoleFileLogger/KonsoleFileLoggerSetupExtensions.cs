using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ConsoleTools.KonsoleFileLogger;

public static class KonsoleFileLoggerSetupExtensions
{

    public static ILoggingBuilder AddKonsoleFile(this ILoggingBuilder builder, Action<KonsoleFileLoggerConfig>? configAction = null)
    {
        var optionsBuilder= builder.Services
            .AddOptions<KonsoleFileLoggerConfig>()
            .BindConfiguration(nameof(KonsoleFileLoggerConfig));
        if (configAction != null) optionsBuilder.Configure(configAction);
        optionsBuilder.ValidateDataAnnotations();

        builder.Services.AddSingleton<KonsoleFileLoggerFilePathProvider>();
        
        builder.Services.AddSingleton<ILoggerProvider, KonsoleFileLoggerProvider>();

        return builder;
    }
}