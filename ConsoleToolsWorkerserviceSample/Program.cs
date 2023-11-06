using ConsoleTools;
using ConsoleTools.KonsoleFileLogger;
using System.Diagnostics;
using System.Text.Json;

namespace ConsoleToolsWorkerserviceTest
{
    public class Program
    {
        public static void Main(string[] args)
        {


            //var konfig = new KonsoleFileLoggerConfig
            //{
            //    Dir = "O:\\MyLogsDeleteMe",
            //    CategoryOverrides = new()
            //    {
            //        new CategoryOverrides
            //        {
            //            CategoryNameBase = "NamedCategory",
            //            FileName = KonsoleFileLoggerConfigConstants.CategoryName
            //        },
            //        new CategoryOverrides
            //        {
            //            CategoryNameBase = "BaseCategory",
            //            FileName = KonsoleFileLoggerConfigConstants.CategorySuffix
            //        }
            //    }
            //};
            //Konsole.PrintJson(JsonSerializer.Serialize(konfig));
            //Konsole.PressAnyKey();




            //по-новому
            var builder = Host.CreateApplicationBuilder();

            builder.Services.AddHostedService<Worker>();
            builder.Services.AddTransient<MyClass>();

            
            builder.Logging.ClearProviders();
            builder.Logging.AddKonsoleFile();

            var host = builder.Build();

            //по-старому
            //IHost host = Host.CreateDefaultBuilder(args)
            //    .ConfigureServices(services => { services.AddHostedService<Worker>(); })
            //    .ConfigureLogging(
            //        logging =>
            //        {
            //            logging.ClearProviders();
            //            logging.AddKonsoleFile();
            //        })
            //    .Build();

            


            //see appsettings.json for logger config
            var logger = host.Services.GetRequiredService<ILogger<Program>>();
            logger!.LogCritical($"It's an ♦m12345 ♦ycritical♦w message from {nameof(Program)} class");
            logger!.LogDebug($"It's a ♦bbla bla ♦ydebug♦y message from {nameof(Program)} class");

            var loggerFactory = host.Services.GetRequiredService<ILoggerFactory>();

            var namedCategoryAbcLogger = loggerFactory.CreateLogger("NamedCategoryAbc");
            namedCategoryAbcLogger.LogCritical($"This is {nameof(namedCategoryAbcLogger)}");

            var namedCategoryXyzLogger = loggerFactory.CreateLogger("NamedCategoryXyz");
            namedCategoryXyzLogger.LogCritical($"This is {nameof(namedCategoryXyzLogger)}");

            var baseCategoryAbcLogger = loggerFactory.CreateLogger("BaseCategoryAbc");
            baseCategoryAbcLogger.LogCritical($"This is {nameof(baseCategoryAbcLogger)}");

            var baseCategoryXyzLogger = loggerFactory.CreateLogger("BaseCategoryXyz");
            baseCategoryXyzLogger.LogCritical($"This is {nameof(baseCategoryXyzLogger)}");

            var baseCategorySubdirLogger = loggerFactory.CreateLogger("BaseCategory\\Subdir\\Xyz");
            baseCategorySubdirLogger.LogCritical($"This is {nameof(baseCategorySubdirLogger)}");

            var myClass = host.Services.GetRequiredService<MyClass>();
            myClass.MyMethod("testing123");


            Konsole.PressAnyKey("♦gLogger has logged♦r.♦g Check console anf file output. Press 'Any' key to launch background worker and make it do more logging...");

            host.Run();
        }
    }

    public class MyClass
    {
        private readonly ILogger<MyClass> _logger;

        public MyClass(ILogger<MyClass> logger)
        {
            _logger = logger;
        }

        public void  MyMethod(string s)
        {
            _logger.LogTraceX($"This is a {nameof(KonsoleFileLoggerExtensions.LogTraceX)}, {s}");
            //await Task.Delay(1000);
            _logger.LogDebugX($"This is a {nameof(KonsoleFileLoggerExtensions.LogDebugX)}, {s}");
            //await Task.Delay(1000);
            _logger.LogInformationX($"This is a {nameof(KonsoleFileLoggerExtensions.LogInformationX)}, {s}");
            //await Task.Delay(1000);
            _logger.LogCriticalX($"This is a {nameof(KonsoleFileLoggerExtensions.LogCriticalX)}, {s}");
            //await Task.Delay(1000);
            _logger.LogWarningX($"This is a {nameof(KonsoleFileLoggerExtensions.LogWarningX)}, {s}");
            //await Task.Delay(1000);
            _logger.LogErrorX($"This is a {nameof(KonsoleFileLoggerExtensions.LogErrorX)}, {s}");
            //await Task.Delay(1000);
        }
    }
}