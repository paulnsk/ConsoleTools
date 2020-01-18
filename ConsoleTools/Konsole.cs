using System;
using System.Reflection;
using System.Threading;



namespace ConsoleTools
{

    /// <summary>
    /// Console.Write() wrapper with simple color support
    /// </summary>
    public static class Konsole
    {
        private static readonly object KlLock = new object();

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
            Write(s + "\n", kolor);
        }


        public static bool CrashOnEscapeKey = true;

        public static void PressAnyKey(string message = "")
        {

            var crashMsg = CrashOnEscapeKey ? " or 'Esc' key to crash" : "";

            if (message != "") WriteLine(message, ConsoleColor.Yellow);
            Write("Press 'Any' key to continue" + crashMsg + "...", ConsoleColor.Yellow);
            var c = Console.ReadKey();
            Console.WriteLine("\n" + c.Key + " was pressed.");
            if (CrashOnEscapeKey && c.KeyChar == 27) throw new Exception("Escape key pressed");
        }

        public static bool Confirmed(string messageWithoutQuestionMark = "")
        {
            if (messageWithoutQuestionMark == "") messageWithoutQuestionMark = "Are you sure";

            Write(messageWithoutQuestionMark, ConsoleColor.Blue);
            Write(" (Y/N)", ConsoleColor.Yellow);
            Write(" ?", ConsoleColor.Blue);
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
        public static string ReadString(string prompt, string defaultValue = "", int maxLength = 20)
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

        public static void PrintObject(object o, string name)
        {

            var col1 = ConsoleColor.Blue;
            var col2 = ConsoleColor.DarkMagenta;

            WriteLine();
            WriteLine(name, ConsoleColor.White);
            foreach (var unused in name)
            {
                Write("-", ConsoleColor.White);
            }

            WriteLine();

            foreach (var prop in o.GetType().GetProperties())
            {
                var value = prop.GetValue(o);
                Write(prop.Name, col1);
                Write(" = ", ConsoleColor.White);
                WriteLine((value ?? " -<null>- ").ToString(), col2);
            }


        }


    }






}
