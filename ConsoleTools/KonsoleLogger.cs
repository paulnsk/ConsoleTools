using System;
using System.IO;

namespace ConsoleTools
{
    /// <summary>
    /// Konsole Logger
    /// </summary>
    public static class KonsoleLogger
    {

        public static LogLevel LogLevel = LogLevel.Full;


        public static bool WriteToFile = true;

        public static string FilePath => U.ExePath() + ".log";

        private static readonly object WriteLock = new object();

        public static void Log(LogLevel ll, string s, ConsoleColor kolor = ConsoleColor.Cyan, bool includeTime = true, bool silent = false)
        {
            if ((int)ll > (int)LogLevel) return;

            lock (WriteLock)
            {
                s += "\n";
                var rightNow = "";
                if (includeTime) rightNow = DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss-fff") + " : ";
                if (!silent)
                {
                    Konsole.Write(rightNow, ConsoleColor.White);
                    Konsole.Write(s, kolor);
                }

                if (!WriteToFile) return;

                try
                {
                    var f = new FileInfo(FilePath);
                    var sw = f.AppendText();
                    sw.Write(rightNow + s);
                    sw.Close();
                }
                catch (Exception e)
                {
                    Konsole.WriteLine("ERROR writing to log file: \n" + "Message: " + s + "\nError: " + e.Message, ConsoleColor.Red);
                }
            }
        }

        public static void LogSilent(LogLevel ll, string s, ConsoleColor kolor = ConsoleColor.Cyan,
            bool includeTime = true)
        {
            Log(ll, s, kolor, includeTime, true);
        }

    }
}