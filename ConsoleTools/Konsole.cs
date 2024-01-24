using System;
using System.Collections.Generic;
using System.Drawing;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading;

namespace ConsoleTools
{

    /// <summary>
    /// Console wrapper with simple color support
    /// </summary>
    public static class Konsole
    {

        public const string EscapeChar = "♦"; //Type Alt+4; 
        
        //todo добавить p=previous
        //todo добавить бэкграунд (как)
        private static Dictionary<char, ConsoleColor> KolorKodes { get; } = new()
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



        private static readonly object KlLock = new();

        public static void Write(string s, ConsoleColor kolor = ConsoleColor.White)
        {
            lock (KlLock)
            {
                foreach (var element in s.ToElements(breakOnEol: false, defaultColor: kolor))
                {
                    Console.ForegroundColor = element.Kolor;
                    Console.Write(element.Text);
                }

                //void WritePiece(string s1, ConsoleColor kolor1)
                //{
                //    Console.ForegroundColor = kolor1;
                //    Console.Write(s1);
                //}

                //if (!string.IsNullOrWhiteSpace(EscapeChar) && s.Contains(EscapeChar))
                //{
                //    var pieces = s.Split(new[] { EscapeChar }, StringSplitOptions.RemoveEmptyEntries);
                //    bool firstPiece = true;
                //    foreach (var piece in pieces)
                //    {
                //        var p = piece;
                //        if (firstPiece && !s.StartsWith(EscapeChar)) p = "_" + piece;
                //        firstPiece = false;
                //        var kolorKode = p[0];
                //        WritePiece(p.Remove(0, 1), KolorKodes.TryGetValue(kolorKode, out var pieceKolor) ? pieceKolor : kolor);
                //    }
                //}
                //else WritePiece(s, kolor);

                //Console.ResetColor();
            }
        }

        public static int CleanLength(string escapedString)
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
        public static string AnyKeyMessage { get; set; } = "Press ♦r'♦gAny♦r'♦= key to continue";
        public static ConsoleColor ConfirmColor { get; set; } = ConsoleColor.Blue;


        public static ConsoleKeyInfo PressAnyKey(string message = "")
        {

            var crashMsg = CrashOnEscapeKey ? " or 'Esc' key to crash" : "";

            if (message != "") WriteLine(message, ConsoleColor.Yellow);
            Write(AnyKeyMessage + crashMsg + "...", ConsoleColor.Yellow);
            var c = Console.ReadKey();
            Console.WriteLine(Environment.NewLine + c.Key + " was pressed.");
            if (CrashOnEscapeKey && c.KeyChar == 27) throw new Exception("Escape key pressed");
            return c;
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

        public static void PrintJson(string json)
        {
            WriteLine(json.ToSyntaxHighlightedJson());
        }

        //_todo make it writeline overload? change writeline to accept objects along with strings?
        public static void PrintObject(object? o)
        {
            WriteLine(o.ToSyntaxHighlightedJson());
        }


        public static void PrintObjectNamed(object o, string? name = default)
        {
            if(string.IsNullOrEmpty(name)) name = o.GetType().Name;
            if (name == null) throw new ArgumentNullException(nameof(name));
            var dic = new Dictionary<string, object>
            {
                [name] = o
            };
            PrintObject(dic);
        }


        public static IEnumerable<KoloredTextElement> ToElements(this string s, bool breakOnEol = false, ConsoleColor defaultColor = ConsoleColor.White)
        {
            if (!string.IsNullOrWhiteSpace(EscapeChar) && s.Contains(EscapeChar))
            {
                //var splitOn = breakOnEol ? new[] { EscapeChar, "\n" } : new[] { EscapeChar };
                var pieces = s.Split(new[] { EscapeChar }, StringSplitOptions.RemoveEmptyEntries);
                var firstPiece = true;
                foreach (var piece in pieces)
                {
                    var p = piece;
                    if (firstPiece && !s.StartsWith(EscapeChar)) p = "_" + piece;
                    firstPiece = false;
                    var kolorKode = p[0];

                    var text = p.Remove(0, 1);
                    var kolor = KolorKodes.TryGetValue(kolorKode, out var pieceKolor) ? pieceKolor : defaultColor;

                    if (!breakOnEol) yield return new KoloredTextElement
                    {
                        Text = text,
                        Kolor = kolor
                    };
                    else
                    {
                        var lines = text.Split('\n');
                        for (var i = 0; i < lines.Length; i++)
                        {
                            yield return new KoloredTextElement
                            {
                                Text = lines[i],
                                Kolor = kolor
                            };
                            if (i < lines.Length - 1)
                                yield return new KoloredTextElement()
                                {
                                    Text = "\n",
                                    Kolor = kolor
                                };
                        }
                    }
                }
            }
            else
                yield return new KoloredTextElement
                {
                    Text = s,
                    Kolor = defaultColor
                };
        }

    }






}
