using System;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleTools
{
    public class MenuItem
    {
        public string Title = "Menu Item";

        public bool ItemBreaksMenuLoop = false;

        public Func<Task> Action = async () =>
        {
            await Task.Run(() =>
            {
                Konsole.WriteLine("\n\nMenu action not specified!", ConsoleColor.Red);
                Thread.Sleep(1000);
            });
        };

        public ConsoleColor Color = ConsoleColor.White;

    }
}