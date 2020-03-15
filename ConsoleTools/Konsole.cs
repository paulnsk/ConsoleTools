using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;



namespace ConsoleTools
{

    /// <summary>
    /// Console.Write() wrapper with simple color support
    /// </summary>
    public static class Konsole
    {

        //todo regions
        

        //todo this should be a stringbuilder
        private static string _toAutoLog = "";

        public static void PurgeLog()
        {
            if (string.IsNullOrWhiteSpace(_toAutoLog)) return;
            KonsoleLogger.Log(LogLevel.LessImportant, _toAutoLog, ConsoleColor.White, true, true);
            _toAutoLog = "";

        }

        private static readonly object KlLock = new object();

        //todo сделать выделение цветом по управляющему символу


        public static void WriteAndLog(string s, ConsoleColor kolor = ConsoleColor.White)
        {
            Write(s, kolor);
            _toAutoLog += s;
        }

        public static void WriteLineAndLog(string s = "", ConsoleColor kolor = ConsoleColor.White)
        {
            WriteLine(s, kolor);
            _toAutoLog += s + Environment.NewLine;
        }



        public static void Write(string s, ConsoleColor kolor = ConsoleColor.White)
        {
            lock (KlLock)
            {
                Console.ForegroundColor = kolor;
                Console.Write(s);
                Console.ResetColor();
            }
        }

        public static void WriteLine(string s = "", ConsoleColor kolor = ConsoleColor.White)
        {
            Write(s + Environment.NewLine, kolor);
        }


        public static bool CrashOnEscapeKey = true;
        public static string AnyKeyMessage = "Press 'Any' key to continue";
        public static ConsoleColor ConfirmColor = ConsoleColor.Blue;

        public static void PressAnyKey(string message = "")
        {

            var crashMsg = CrashOnEscapeKey ? " or 'Esc' key to crash" : "";

            if (message != "") WriteLine(message, ConsoleColor.Yellow);
            Write(AnyKeyMessage + crashMsg + "...", ConsoleColor.Yellow);
            var c = Console.ReadKey();
            Console.WriteLine(Environment.NewLine + c.Key + " was pressed.");
            if (CrashOnEscapeKey && c.KeyChar == 27) throw new Exception("Escape key pressed");
        }

        public static bool Confirmed(string messageWithoutQuestionMark = "")
        {
            if (messageWithoutQuestionMark == "") messageWithoutQuestionMark = "Are you sure";

            Write(messageWithoutQuestionMark, ConfirmColor);
            Write(" (Y/N)", ConsoleColor.Yellow);
            Write(" ?", ConfirmColor);
            var answer = Console.ReadKey().KeyChar.ToString().ToLower();
            var result = answer == "y";
            if (result) Write(" (Yes)", ConsoleColor.Green);
            else Write(" (No)", ConsoleColor.Red);
            Thread.Sleep(500);
            return result;
        }

        /// <summary>
        /// Lauches KonsoleStringEdit with default params. 
        /// </summary>
        /// <param name="prompt"></param>
        /// <param name="defaultValue"></param>
        /// <param name="maxLength"></param>
        /// <returns></returns>
        public static string ReadString(string prompt, string defaultValue = "", int maxLength = 20, bool isPassword=false)
        {
            return new KonsoleStringEdit
            {
                Prompt = prompt, DefaultValue = defaultValue, MaxLength = maxLength,
                EscapeBehavior = KonsoleStringEditEscapeBehavior.ReturnEmptystring
            }.ReadString();
        }

        /// <summary>
        /// Reads all keys from keyboard buffer and returns true if at least one character matches ch. Ignores case
        /// </summary>
        /// <returns></returns>
        public static bool CheckForKey(char ch)
        {
            var result = false;

            while (Console.KeyAvailable)
            {
                var k = Console.ReadKey();
                if (string.Equals(k.KeyChar.ToString(), ch.ToString(), StringComparison.CurrentCultureIgnoreCase))
                    result = true; //keep reading until the buffer is empty
            }

            return result;
        }


        //todo отключать log надо б как-то
        public static void PrintObject(object o, string name, int indent = 0)
        {

            var exclude = new[] { typeof(string), typeof(DateTime) };

            IEnumerable<PropertyInfo> Props(object x) //without indexers
            {
                return x.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public).Where(p => p.GetIndexParameters().Length == 0);
            }
            
            //has properties and is not in the exclude list
            bool NeedsRecursive(object x)
            {
                return !exclude.Contains(x.GetType()) && Props(x).Any();
            }

            void DoIndent()
            {
                for (var i = 0; i < indent; i++) { WriteAndLog(" "); }
            }


            var col1 = ConsoleColor.Yellow;
            var col2 = ConsoleColor.DarkCyan;

            //WriteLineAndLog();
            DoIndent();
            WriteAndLog(name);
            WriteAndLog(" <", ConsoleColor.Yellow);
            WriteAndLog(o.GetType().ToString(), ConsoleColor.DarkGray);
            WriteLineAndLog(">", ConsoleColor.Yellow);
            

            foreach (var prop in Props(o))
            {
                var value = prop.GetValue(o);

                if (NeedsRecursive(value)) PrintObject(value, name + "." + prop.Name, indent + 2);
                else
                {
                    DoIndent();

                    WriteAndLog(". " + prop.Name, col1);
                    WriteAndLog(" = ", ConsoleColor.White);
                    WriteLineAndLog((value ?? " -<null>- ").ToString(), col2);
                }
            }
            if (o is IEnumerable<object> valueItems)
            {
                var i = 0;
                foreach (var item in valueItems)
                {
                    PrintObject(item, name + $"[{i}]", indent + 2);
                    i++;
                }
            }


        }


    }






}
