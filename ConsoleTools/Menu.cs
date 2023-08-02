using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ConsoleTools
{
    public class Menu
    {

        //Menu items are invoked by pressing number keys therefore only single digit numbers are possible
        //public static int MaxItems = 9; 

        public static string PressNumberMessage = "Press number or Q to exit: ";

        public Menu(string title)
        {
            _title = title;
        }

        public Action BeforeDisplay { get; set; } = null;

        public List<MenuItem> Items;

        public ConsoleColor HotkeyColor { get; set; } = ConsoleColor.DarkYellow;
        public ConsoleColor PromptColor { get; set; } = ConsoleColor.Cyan;

        public delegate void DefaultExceptionHandlerAction(Exception exception, MenuItem menuItem);

        public DefaultExceptionHandlerAction DefaultExceptionHandler { get; set; } = (e, mi) =>
        {
            Konsole.WriteLine();
            Konsole.WriteLine();
            Konsole.WriteLine($"♦RERROR ♦wexecuting command [♦b{mi.Title}♦w]:\n♦w{e.ToStringWithInners()}");
            Konsole.WriteLine();
            Konsole.WriteLine();
            Konsole.PressAnyKey();
        };

        public bool DefaultExceptionHandlerEnabled { get; set; } = true;

        private readonly string _title;

        private void Display()
        {
            Console.Clear();

            BeforeDisplay?.Invoke();
            
            //if (Items.Count > 9) Konsole.WriteLine("Too many menu items!!", ConsoleColor.Red);
            
            Konsole.WriteLine(_title, HotkeyColor);
            foreach (var unused in _title) { Konsole.Write("`",HotkeyColor); }
            Console.WriteLine();
            
            for (var i = 0; i < Items.Count; i++)
            {
                Konsole.Write(NumberToHotChar(i + 1) + " ", HotkeyColor);
                Konsole.WriteLine(Items[i].Title);
            }

            Console.WriteLine();

        }



        /// <summary>
        /// converts 5 to  '5', 15 to 'f'
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        private string NumberToHotChar(int number)
        {
            if (number < 10) return number.ToString();
            return ((char) (number + 87)).ToString();
        }


        /// <summary>
        /// Converts '1' to 1, 'a' to 10
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        private int HotCharToNumber(char c)
        {
            var code = (int)c.ToString().ToLower()[0];
            if (code > (int)'9') code -= 87;
            else code -= 48;
            return code;
        }


        public async Task Loop()
        {
            
            while (true)
            {
                
                Display();
                Konsole.Write(PressNumberMessage, PromptColor);

                var k = Console.ReadKey();
                if(k.KeyChar==(char)27 || k.KeyChar.ToString().ToLower() == "q") break;

                var itemNumber = HotCharToNumber(k.KeyChar) - 1;
                //var itemNumber = k.KeyChar.ToString().ToInt() - 1;
                
                if (itemNumber >= 0 && itemNumber < Items.Count)
                {
                    try
                    {
                        await Items[itemNumber].Action();
                    }
                    catch (Exception e)
                    {
                        if (DefaultExceptionHandlerEnabled) DefaultExceptionHandler(e, Items[itemNumber]);
                        else throw;
                    }
                    
                    if (Items[itemNumber].ItemBreaksMenuLoop) break;
                }
            }
            
        }
    }
}