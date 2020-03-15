using System;
using System.Linq;
using System.Text;

namespace ConsoleTools
{
 
    /// <summary>
    /// A smarter alternative for Console.ReadLine. Allows default value, supports cursor keys etc. Some keys are not handled
    /// </summary>
    public class KonsoleStringEdit
    {
        
        public int MaxLength { get; set; } = -1;
        public string DefaultValue { get; set; } = "";
        public ConsoleColor ValueColor { get; set; } = ConsoleColor.Yellow;
        public ConsoleColor PromptColor { get; set; } = ConsoleColor.White;
        public bool LineFeed { get; set; } = true;
        public string Prompt { get; set; } = "";
        public bool ShowRedEscapeInPrompt { get; set; } = false;
        public bool IsPassword { get; set; }

        public KonsoleStringEditEscapeBehavior EscapeBehavior { get; set; } =
            KonsoleStringEditEscapeBehavior.ReturnEmptystring;


        private string RepeatChars(string chr, int count)
        {
            var sb = new StringBuilder();
            for (var i = 0; i < count; i++) {sb.Append(chr); }
            return sb.ToString();
        }

        private string Asterize(string s)
        {
            if (!IsPassword) return s;
            return RepeatChars("*", s.Length);
        }


        private void GoLeft()
        {
            if (_pos < _value.Length) _pos++;
            Redraw();
        }

        private void GoRight()
        {
            if (_pos > 0) _pos--;
            Redraw();
        }


        private void GoHome()
        {
            _pos = 0;
            Redraw();
        }

        private void GoEnd()
        {
            _pos = _value.Length;
            Redraw();
        }
        
        private void Redraw()
        {
            Console.SetCursorPosition(_savedX, _savedY);
            Konsole.Write(Asterize(_value) + " ", ValueColor); //затираем лишнее
            Console.SetCursorPosition(_savedX + _pos, _savedY);
        }

        private void Backspace()
        {
            if (_pos > 0)
            {
                _value = _value.Remove(_pos - 1, 1);
                _pos--;
            }
            Redraw();
        }

        private void Delete()
        {
            if (_pos < _value.Length) _value = _value.Remove(_pos, 1);
            Redraw();
        }

        private void Insert(char c)
        {
            if (_value.Length < MaxLength)
            {
                _value = _value.Insert(_pos, c.ToString());
                _pos++;
            }
            Redraw();
        }


        private void Clear()
        {
            var empty = _value.Aggregate("", (current, unused) => current + " "); //replacing value with a string of the same length made of spaces
            _value = empty;
            Redraw();
            _value = "";
            _pos = 0;
            Redraw();
        }


        private void WritePrompt()
        {
            if (string.IsNullOrEmpty(Prompt)) return;
            var semicolon = false;
            var s = Prompt;
            if (ShowRedEscapeInPrompt && EscapeBehavior == KonsoleStringEditEscapeBehavior.ReturnEmptystring)
            {
                if (s.EndsWith(":"))
                {
                    semicolon = true;
                    s = s.Remove(s.Length - 1);
                }
            }
            Konsole.Write(s, PromptColor);
            if (ShowRedEscapeInPrompt && EscapeBehavior == KonsoleStringEditEscapeBehavior.ReturnEmptystring)
            {
                Konsole.Write(" [Esc]", ConsoleColor.Red);
            }
            if (semicolon) Konsole.Write(":", PromptColor);
            //Konsole.Write(" ");
        }

        private int _pos;
        private string _value;
        private int _savedX = Console.CursorLeft;
        private int _savedY = Console.CursorTop;

        public string ReadString()
        {
            _value = DefaultValue;
            _pos = _value.Length;

            WritePrompt();
            


            _savedX = Console.CursorLeft;
            _savedY = Console.CursorTop;
            Konsole.Write(Asterize(_value), ValueColor);

            while (true)
            {
                var c = Console.ReadKey(true);
                switch (c.Key)
                {
                    case ConsoleKey.Enter:
                    {
                        if (LineFeed) Console.WriteLine();
                        return _value;
                    }
                    case ConsoleKey.Escape:
                        
                        Console.Write("A");//гасим escape последовательность, а то она норовит сожрать любой символ,выводимый после esc. Причем пробелом и знаками препинания не гасится
                        
                        switch (EscapeBehavior)
                        {
                            case KonsoleStringEditEscapeBehavior.ReturnEmptystring:
                                if (LineFeed) Console.WriteLine();
                                return "";
                            case KonsoleStringEditEscapeBehavior.ClearString:
                                Clear();
                                break;
                            case KonsoleStringEditEscapeBehavior.ReturnEsc:
                                if (LineFeed) Console.WriteLine();
                                return "esc";
                            case KonsoleStringEditEscapeBehavior.Ignore:
                                break;
                            default:
                                throw new ArgumentOutOfRangeException();
                        }
                        break;
                    case ConsoleKey.LeftArrow:
                    case ConsoleKey.UpArrow:
                        GoRight();
                        break;
                    case ConsoleKey.RightArrow:
                    case ConsoleKey.DownArrow:
                        GoLeft();
                        break;
                    case ConsoleKey.Home:
                        GoHome();
                        break;
                    case ConsoleKey.End:
                        GoEnd();
                        break;
                    case ConsoleKey.Backspace:
                        Backspace();
                        break;
                    case ConsoleKey.Delete:
                        Delete();
                        break;
                    default:
                        Insert(c.KeyChar);
                        break;
                }
            }
        }
    }

    public enum KonsoleStringEditEscapeBehavior
    {
        ReturnEmptystring,
        ClearString,
        ReturnEsc,
        Ignore
    }

    

}