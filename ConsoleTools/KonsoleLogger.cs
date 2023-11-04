using System;
using System.IO;

namespace ConsoleTools
{
    /// <summary>
    /// A simple standalone logger which writes to both Konsole and file simultaneously
    /// </summary>
    public static class KonsoleLogger
    {

        public static LogLevel LogLevel = LogLevel.Full;
        
        public static bool WriteToFile = true;

        public static string FilePath => Utils.ExePath() + ".log";

        private static readonly object WriteLock = new();

        public static void Log(MessageLevel ml, string s, ConsoleColor kolor = ConsoleColor.Cyan, bool includeTime = true, bool silent = false)
        {
            if ((int)ml > (int)LogLevel) return;

            lock (WriteLock)
            {
                s += Environment.NewLine;
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
                    sw.Write(rightNow + Konsole.UnEscape(s));
                    sw.Close();
                }
                catch (Exception e)
                {
                    Konsole.WriteLine("ERROR writing to log file: " + Environment.NewLine + "Message: " + s + Environment.NewLine + "Error: " + e.Message, ConsoleColor.Red);
                }
            }
        }


        public static void Log(string s, ConsoleColor kolor = ConsoleColor.Cyan)
        {
            Log(MessageLevel.LessImportant, s, kolor);
        }

        public static void LogSilent(MessageLevel ml, string s, ConsoleColor kolor = ConsoleColor.Cyan,
            bool includeTime = true)
        {
            Log(ml, s, kolor, includeTime, true);
        }

    }
}