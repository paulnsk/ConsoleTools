using System;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleTools
{
    public class MenuItem
    {
        public string Title { get; set; } = "Menu Item";

        public bool ItemBreaksMenuLoop { get; set; } = false;
        public bool? PressAnyKeyAfterAction { get; set; }
        public bool? WriteLineBeforeAction { get; set; }

        public Func<Task> Action { get; set; } = async () =>
        {
            await Task.Run(() =>
            {
                Konsole.WriteLine(Environment.NewLine + Environment.NewLine + "Menu action not specified!", ConsoleColor.Red);
                Thread.Sleep(1000);
            });
        };

        public ConsoleColor Color { get; set; } = ConsoleColor.White;
    }
}