using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace ConsoleTools;

/// <summary>
/// Console wrapper with simple color support
/// </summary>
public static class Konsole
{

    public static string[] EscapeChars = ["♦","♣"]; //Type Alt+4 for ♦, Alt+5 for ♣; 
        
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

    public static event EventHandler<KoloredTextElement>? PrintElement;

    public static void Write(string s, ConsoleColor kolor = ConsoleColor.White)
    {
        lock (KlLock)
        {
            foreach (var element in s.ToElements(breakOnEol: false, defaultColor: kolor))
            {
                try
                {
                    //not supported on this platform error in avalonia wasm
                    Console.ForegroundColor = element.Kolor;
                }
                catch (Exception)
                {
                    // ignored
                }

                Console.Write(element.Text);
                PrintElement?.Invoke(null, element);
            }

            if (_printToHtml)
            {
                foreach (var element in s.ToElements(breakOnEol: false, defaultColor: kolor))
                {
                    AddFragmentToHtml(element.Text, element.Kolor);
                }
            }
        }
    }

    public static int CleanLength(string escapedString)
    {
        var escapeCount = escapedString.Count(x => EscapeChars.Contains(x.ToString()));
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
    public static string AnyKeyMessage { get; set; } = "Press ♦r'♦gAny♦r'♦= key";
    public static ConsoleColor ConfirmColor { get; set; } = ConsoleColor.Blue;


    public static ConsoleKeyInfo PressAnyKey(string message = "", bool? replaceAnykeyMessage = true)
    {
        var crashMsg = CrashOnEscapeKey ? " or 'Esc' key to crash" : "";
        
        if (message != "") Write(message);
        if (replaceAnykeyMessage == false || string.IsNullOrWhiteSpace(message)) Write(Environment.NewLine + AnyKeyMessage + crashMsg + "...", ConsoleColor.Yellow);
        var c = Console.ReadKey();
        WriteLine("♦=(♦g" + c.Key + "♦=)");
        if (CrashOnEscapeKey && c.KeyChar == 27) throw new Exception("Escape key pressed");
        return c;
    }

    public static ConsoleKeyInfo PressAnyKeyTo(string toDoWhat)
    {
        return PressAnyKey("♣y" + AnyKeyMessage + "♣y to ♣c" + toDoWhat+"♣y...");
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

    public static bool ConfirmedLine(string messageWithoutQuestionMark = "")
    {
        var result = Confirmed(messageWithoutQuestionMark);
        WriteLine();
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
        for (var i = 0; i < s.Length; i++)
        {
            if (EscapeChars.Contains(s[i].ToString()))
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
        // Check if EscapeChars array is not null or empty and if s contains any escape character
        if (EscapeChars.Length > 0 && EscapeChars.Any(ec => s.Contains(ec)))
        {
            // Split the string using any of the escape characters from the array
            var pieces = s.Split(EscapeChars, StringSplitOptions.RemoveEmptyEntries);
            var firstPiece = true;
            foreach (var piece in pieces)
            {
                var p = piece;
                // If the string doesn't start with any escape character, prepend an underscore
                if (firstPiece && !EscapeChars.Any(ec => s.StartsWith(ec)))
                    p = "_" + piece;
                firstPiece = false;
                var kolorKode = p[0];

                var text = p.Remove(0, 1);
                var kolor = KolorKodes.TryGetValue(kolorKode, out var pieceKolor) ? pieceKolor : defaultColor;

                if (!breakOnEol)
                    yield return new KoloredTextElement
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
                            yield return new KoloredTextElement
                            {
                                Text = "\n",
                                Kolor = kolor
                            };
                    }
                }
            }
        }
        else
        {
            yield return new KoloredTextElement
            {
                Text = s,
                Kolor = defaultColor
            };
        }
    }


    //public static IEnumerable<KoloredTextElement> ToElements(this string s, bool breakOnEol = false, ConsoleColor defaultColor = ConsoleColor.White)
    //{
    //    if (!string.IsNullOrWhiteSpace(EscapeChar) && s.Contains(EscapeChar))
    //    {
    //        //var splitOn = breakOnEol ? new[] { EscapeChar, "\n" } : new[] { EscapeChar };
    //        var pieces = s.Split(new[] { EscapeChar }, StringSplitOptions.RemoveEmptyEntries);
    //        var firstPiece = true;
    //        foreach (var piece in pieces)
    //        {
    //            var p = piece;
    //            if (firstPiece && !s.StartsWith(EscapeChar)) p = "_" + piece;
    //            firstPiece = false;
    //            var kolorKode = p[0];

    //            var text = p.Remove(0, 1);
    //            var kolor = KolorKodes.TryGetValue(kolorKode, out var pieceKolor) ? pieceKolor : defaultColor;

    //            if (!breakOnEol) yield return new KoloredTextElement
    //            {
    //                Text = text,
    //                Kolor = kolor
    //            };
    //            else
    //            {
    //                var lines = text.Split('\n');
    //                for (var i = 0; i < lines.Length; i++)
    //                {
    //                    yield return new KoloredTextElement
    //                    {
    //                        Text = lines[i],
    //                        Kolor = kolor
    //                    };
    //                    if (i < lines.Length - 1)
    //                        yield return new KoloredTextElement()
    //                        {
    //                            Text = "\n",
    //                            Kolor = kolor
    //                        };
    //                }
    //            }
    //        }
    //    }
    //    else
    //        yield return new KoloredTextElement
    //        {
    //            Text = s,
    //            Kolor = defaultColor
    //        };
    //}

    //perhaps support more types? no idea
    public static string Kolorify(this bool b)
    {
        return b ? $"♦g{b}" : $"♦r{b}";
    }

    private static bool _printToHtml;
    public static bool PrintToHtml
    {
        get => _printToHtml;
        set
        {
            lock (KlLock)
            {
                _printToHtml = value;
                if (_printToHtml && HtmlBuffer.Count == 0)
                {
                    // Initialize the HTML structure
                    HtmlBuffer.Add("<!DOCTYPE html>");
                    HtmlBuffer.Add("<html>");
                    HtmlBuffer.Add("<head><style>");
                    HtmlBuffer.Add("body { background-color: black; color: white; font-family: monospace; }");
                    HtmlBuffer.Add("</style></head>");
                    HtmlBuffer.Add("<body>");
                }
            }
        }
    }

    private static void AddFragmentToHtml(string text, ConsoleColor color)
    {
        string colorHex = ColorToHex(color);
        HtmlBuffer.Add($"<span style=\"color:{colorHex}; white-space: pre; line-height: 0.6;\">{System.Web.HttpUtility.HtmlEncode(text)}</span>");
    }

    public static string GetPrintedHtml()
    {
        lock (KlLock)
        {
            if (HtmlBuffer.Count == 0)
                return string.Empty;

            var resultBuffer = new List<string>(HtmlBuffer)
            {
                "</body>",
                "</html>"
            };

            return string.Join("", resultBuffer);

        }
    }

    public static void ResetHtml()
    {
        HtmlBuffer.Clear();
    }
    

    private static readonly List<string> HtmlBuffer = new List<string>();

    private static string ColorToHex(ConsoleColor color)
    {
        return color switch
        {
            ConsoleColor.Black => "#000000",
            ConsoleColor.DarkBlue => "#00008B",
            ConsoleColor.DarkGreen => "#006400",
            ConsoleColor.DarkCyan => "#008B8B",
            ConsoleColor.DarkRed => "#8B0000",
            ConsoleColor.DarkMagenta => "#8B008B",
            ConsoleColor.DarkYellow => "#B8860B",
            ConsoleColor.Gray => "#808080",
            ConsoleColor.DarkGray => "#A9A9A9",
            ConsoleColor.Blue => "#0000FF",
            ConsoleColor.Green => "#008000",
            ConsoleColor.Cyan => "#00FFFF",
            ConsoleColor.Red => "#FF0000",
            ConsoleColor.Magenta => "#FF00FF",
            ConsoleColor.Yellow => "#FFFF00",
            ConsoleColor.White => "#FFFFFF",
            _ => "#FFFFFF"
        };
    }



}