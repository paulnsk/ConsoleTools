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
        
        builder.Services.AddSingleton<KonsoleFileLoggerProvider>();
        //for some reason simply registering logger provider as singleton it not enough so we have to force it to be created and added to AddProviders:

        builder.AddProvider(builder.Services.BuildServiceProvider().GetRequiredService<KonsoleFileLoggerProvider>());
        
        return builder;
    }
}