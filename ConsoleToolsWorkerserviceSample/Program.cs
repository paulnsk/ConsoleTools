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

            Konsole.PressAnyKey("♦gLogger has logged♦r.♦g Check console anf file output. Press 'Any' key to launch background worker and make it do more logging...");

            host.Run();
        }
    }
}