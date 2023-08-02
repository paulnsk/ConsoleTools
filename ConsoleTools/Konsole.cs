﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;



namespace ConsoleTools
{

    /// <summary>
    /// Console wrapper with simple color support
    /// </summary>
    public static class Konsole
    {

        public const string EscapeChar = "♦"; //Type Alt+4; 

        public static readonly Dictionary<char, ConsoleColor> KolorKodes = new Dictionary<char, ConsoleColor>
        {
            ['k'] = ConsoleColor.Black,
            ['B'] = ConsoleColor.DarkBlue,
            ['G'] = ConsoleColor.DarkGreen,
            ['C'] = ConsoleColor.DarkCyan,
            ['R'] = ConsoleColor.DarkRed,
            ['M'] = ConsoleColor.DarkMagenta,
            ['Y'] = ConsoleColor.DarkYellow,
            ['a'] = ConsoleColor.Gray,
            ['A'] = ConsoleColor.DarkGray,
            ['b'] = ConsoleColor.Blue,
            ['g'] = ConsoleColor.Green,
            ['c'] = ConsoleColor.Cyan,
            ['r'] = ConsoleColor.Red,
            ['m'] = ConsoleColor.Magenta,
            ['y'] = ConsoleColor.Yellow,
            ['w'] = ConsoleColor.White
        };



        private static readonly object KlLock = new object();

        public static void Write(string s, ConsoleColor kolor = ConsoleColor.White)
        {
            lock (KlLock)
            {

                void WritePiece(string s1, ConsoleColor kolor1)
                {
                    Console.ForegroundColor = kolor1;
                    Console.Write(s1);
                }

                if (!string.IsNullOrWhiteSpace(EscapeChar) && s.Contains(EscapeChar))
                {
                    var pieces = s.Split(new[] { EscapeChar }, StringSplitOptions.RemoveEmptyEntries);
                    bool firstPiece = true;
                    foreach (var piece in pieces)
                    {
                        var p = piece;
                        if (firstPiece && !s.StartsWith(EscapeChar)) p = "_" + piece;
                        firstPiece = false;
                        var kolorKode = p[0];
                        WritePiece(p.Remove(0, 1), KolorKodes.TryGetValue(kolorKode, out var pieceKolor) ? pieceKolor : kolor);
                    }
                }
                else WritePiece(s, kolor);

                Console.ResetColor();
            }
        }

        private static int CleanLength(string escapedString)
        {
            var escapeCount = escapedString.Count(x => x.ToString() == EscapeChar);
            return escapedString.Length - escapeCount * 2;
        }

        public static void WriteLineUnderlined(string s = "", ConsoleColor kolor = ConsoleColor.White, char underlineChar='-')
        {
            WriteLine(s, kolor);
            for (var i = 0; i < CleanLength(s); i++)
            {
                Write(underlineChar.ToString(), kolor);
            }
            WriteLine();
        }

        public static void WriteLine(string s = "", ConsoleColor kolor = ConsoleColor.White)
        {
            Write(s + Environment.NewLine, kolor);
        }


        public static bool CrashOnEscapeKey { get; set; } = false;
        public static string AnyKeyMessage { get; set; } = "Press 'Any' key to continue";
        public static ConsoleColor ConfirmColor { get; set; } = ConsoleColor.Blue;


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
        /// <param name="isPassword"></param>
        /// <returns></returns>
        public static string ReadString(string prompt, string defaultValue = "", int maxLength = 20, bool isPassword=false)
        {
            return new KonsoleStringEdit
            {
                Prompt = prompt, 
                DefaultValue = defaultValue, 
                MaxLength = maxLength,
                EscapeBehavior = KonsoleStringEditEscapeBehavior.ReturnEmptystring,
                IsPassword = isPassword
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

        public static string UnEscape(string s)
        {
            var sb = new StringBuilder();
            for (int i = 0; i < s.Length; i++)
            {
                if (s[i].ToString() == Konsole.EscapeChar)
                {
                    i += 1;
                    continue;
                }

                sb.Append(s[i]);
            }

            return sb.ToString();

        }
        

    }






}
