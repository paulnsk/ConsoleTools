using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

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
                    sw.Write(rightNow + ClearEscape(s));
                    sw.Close();
                }
                catch (Exception e)
                {
                    Konsole.WriteLine("ERROR writing to log file: " + Environment.NewLine + "Message: " + s + Environment.NewLine + "Error: " + e.Message, ConsoleColor.Red);
                }
            }
        }

        private static string ClearEscape(string s)
        {
            var sb = new StringBuilder();
            for (int i = 0; i < s.Length; i++)
            {
                if (s[i].ToString() == Konsole.EscapeChar)
                {
                    i+=1;
                    continue;
                }

                sb.Append(s[i]);
            }

            return sb.ToString();
            ////не работает, сжирает первую букву у которой нет искейпчара
            //var pieces = s.Split(new[] { Konsole.EscapeChar }, StringSplitOptions.None);
            //var cleanPieces = new List<string>();
            //foreach (var piece in pieces)
            //{
            //    if (piece.Length > 0) cleanPieces.Add(piece.Remove(0, 1));
            //}

            //return string.Join("", cleanPieces);
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