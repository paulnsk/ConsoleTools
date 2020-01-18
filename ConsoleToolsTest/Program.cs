using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using ConsoleTools;
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
            


            //Console.WriteLine("\n\n");
            //Konsole.WriteLine(
            //    "WARNING: if you delete all users you will have to manually copy new security.json file to solr data dir in order to restore access",
            //    ConsoleColor.DarkRed);
            //if (!Konsole.Confirmed("Continue?")) return;
            //Console.WriteLine(); Console.WriteLine();
            Console.WriteLine();

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
                            Konsole.WriteLine(x.GetType().ToString());

                            var props = x.GetType().GetProperties().Where(p => p.GetIndexParameters().Length == 0);
                            Konsole.WriteLine(props.Count().ToString());

                            foreach (var info in props)
                            {
                                Konsole.WriteLine(info.Name);
                                var value = info.GetValue(x);
                                Konsole.WriteLine(value.ToString());
                            }

                            Konsole.PressAnyKey();
                        });
                    }},
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


