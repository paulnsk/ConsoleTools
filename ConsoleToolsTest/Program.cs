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
            return;

            //string sss = Konsole.ReadString("Enter something: ", "abcd", 20, true);
            //Konsole.PressAnyKey(sss);
            //return;

            //var userName = Konsole.ReadString("Enter user name:", "").Trim();
            //if (string.IsNullOrWhiteSpace(userName))
            //{
            //    Konsole.PressAnyKey("You did not enter name");
            //    return;
            //}


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
                        Title = "One",
                        Action = async () =>
                        {

                            var keepWaiting = true;

                            var t = new Timer {Interval = 2000};
                            t.Elapsed += (s1, e1) =>
                            {
                                Konsole.WriteLineAndLog("\n\nHello from timer... Press Z to stop waiting for events and return to menu");
                                Konsole.PurgeLog();
                                while (Console.KeyAvailable)
                                {
                                    var k = Console.ReadKey();
                                    if (k.KeyChar.ToString().ToLower() == "Z") keepWaiting = false;
                                }
                            };
                            t.Start();

                            Konsole.WriteLine("\nWaiting for events...");

                            while (keepWaiting)
                            {
                                Konsole.WriteLine("waiting.....");
                                Thread.Sleep(100);
                            }

                            //Konsole.PressAnyKey();
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
                            Konsole.PurgeLog();
                        });
                    }},
                    new MenuItem(){Title = "Print simple", Action = async () =>
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


