using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ConsoleTools
{
    public class Menu
    {

        //Menu items are invoked by prssing number keys therefore only single digit numbers are possible
        public static int MaxItems = 9; 

        

        public Menu(string title)
        {
            _title = title;
        }

        public Action BeforeDisplay = null;

        public List<MenuItem> Items;

        public ConsoleColor HotkeyColor = ConsoleColor.DarkYellow;
        public ConsoleColor PromptColor = ConsoleColor.Cyan;
        private string _title;

        public void Display()
        {
            Console.Clear();

            BeforeDisplay?.Invoke();
            
            if (Items.Count > 9) Konsole.WriteLine("Too many menu items!!", ConsoleColor.Red);
            
            Konsole.WriteLine(_title, HotkeyColor);
            foreach (var unused in _title) { Konsole.Write("`",HotkeyColor); }
            Console.WriteLine();


            for (var i = 0; i < Items.Count; i++)
            {
                Konsole.Write((i + 1) + " ", HotkeyColor);
                Konsole.WriteLine(Items[i].Title);
            }

            Console.WriteLine();

        }
        

        public async Task Loop()
        {
            
            while (true)
            {
                
                Display();
                Konsole.Write("Press number or Q to exit: ", PromptColor);

                var k = Console.ReadKey();
                if(k.KeyChar==(char)27 || k.KeyChar.ToString().ToLower() == "q") break;

                var itemNumber = k.KeyChar.ToString().ToInt() - 1;
                

                if (itemNumber >= 0 && itemNumber < Items.Count)
                {
                    await Items[itemNumber].Action();
                }
            }
            
        }
    }
}