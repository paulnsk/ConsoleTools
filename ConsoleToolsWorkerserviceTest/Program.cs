using ConsoleTools;
using ConsoleTools.KonsoleFileLogger;
using System.Diagnostics;

namespace ConsoleToolsWorkerserviceTest
{
    public class Program
    {
        public static void Main(string[] args)
        {
            //по-новому
            var builder = Host.CreateApplicationBuilder();
            builder.Services.AddHostedService<Worker>();

            var exepath = Process.GetCurrentProcess().MainModule?.FileName;
            var exedir = Path.GetDirectoryName(exepath);
            var exeFile = Path.GetFileNameWithoutExtension(exepath);
            var logFilePath = Path.Combine(exedir!, "LOG_" + exeFile + ".log");

            builder.Logging.ClearProviders();
            builder.Logging.AddKonsoleFile(logFilePath, deleteOldLog: true);

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


            var logger = host.Services.GetService<ILogger<Program>>();
            logger!.LogCritical("It's an ♦m12345 ♦ycritical♦w message");
            logger!.LogDebug("It's a ♦bbla bla ♦ydebug♦y message");

            host.Run();
        }
    }
}