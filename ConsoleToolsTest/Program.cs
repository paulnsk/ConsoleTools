using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using ConsoleTools;
using Kl = ConsoleTools.KonsoleLogger;
using Timer = System.Timers.Timer;

namespace ConsoleToolsTest
{
    public static class Program
    {
        private static async Task Main(string[] args)
        {


            //while (true)
            //{
            //    Konsole.PressAnyKey();
            //}


            

            Konsole.CrashOnEscapeKey = false;
            


            
            
            Console.WriteLine();
            var x = "123";
            Kl.Log($"Test ♦g{x}",ConsoleColor.Yellow);

            //string sss = Konsole.ReadString("Enter something: ", "abcd", 20, true);
            //Konsole.PressAnyKey(sss);
            //return;


            //while (true)
            //{
            //    //Konsole.WriteLine($"[{new KonsoleStringEdit {Prompt ="Введи строку:", DefaultValue = "Dефаульт", MaxLength = 15, EscapeBehavior = KonsoleStringEditEscapeBehavior.ReturnEmptystring}.ReadString()}]");
            //    var s = Konsole.ReadString("Enter something:", "something");
            //    Konsole.WriteLine($"[{s}]");
            //}


            var menu = new Menu("Main")
            {
                Items = new List<MenuItem>()
                {
                    new MenuItem()
                    {
                        Title = "String editor example",
                        Action = async () =>
                        {
                            var s = Konsole.ReadString("\n\nString editor:", "edit me").Trim();
                            if (string.IsNullOrWhiteSpace(s))
                            {
                                Konsole.PressAnyKey("You did not enter anything");
                                return;
                            }
                            Konsole.PressAnyKey($"You entered [{s}]");
                        }
                    },
                    new MenuItem
                    {Title="Print object", Action = async () =>
                    {
                        var aPerson = new Person {Name = "Wassily Pupeking", Age = 888, Details = new PersonDetails(){Height = 123, SchoolName = "Asdfg", DetailedDetails = new DetailedDetails(){DetailOne = "One", DetailTwo = 2}}};
                        await Task.Run(() =>
                        {
                            Konsole.PrintObject(aPerson, nameof(aPerson));
                            Konsole.PressAnyKey();
                        });
                    }},
                    new MenuItem(){Title = "Print another object", Action = async () =>
                    {
                        var x = "test";
                        await Task.Run(() =>
                        {
                            Konsole.PrintObject(x,"X");

                            Konsole.PressAnyKey();
                        });
                    }},
                    new MenuItem(),
                    new MenuItem(),
                    new MenuItem(),
                    new MenuItem(),
                    new MenuItem(),
                    new MenuItem(),
                    new MenuItem(),
                    new MenuItem(),
                    new MenuItem(),
                    new MenuItem(),
                    new MenuItem(),
                    new MenuItem()
                }
            };

            await menu.Loop();

        }
    }

    class Person
    {
        public string Name { get; set; }
        public int Age { get; set; }
        public PersonDetails Details { get; set; }
    }


    class PersonDetails
    {
        public int Height { get; set; }
        public string SchoolName { get; set; }
        public DetailedDetails DetailedDetails { get; set; }
    }

    class DetailedDetails
    {
        public string DetailOne { get; set; }
        public int DetailTwo { get; set; }
    }


}


