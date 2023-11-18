using System.Collections.Generic;
using System.Threading.Tasks;
using ConsoleTools;

namespace ConsoleToolsSamples
{
    public static class Program
    {
        private static async Task Main(string[] args)
        {
            
            Konsole.CrashOnEscapeKey = false;

            var menu = new Menu("Main")
            {
                Items = new List<MenuItem>()
                {
                    new()
                    {
                        Title = "String editor example",
                        Action = async () =>
                        {
                            var s = Konsole.ReadString("\n\n Enter your name:", "Jhon Smith").Trim();
                            if (string.IsNullOrWhiteSpace(s))
                            {
                                Konsole.PressAnyKey("You did not enter anything");
                                return;
                            }

                            Konsole.PressAnyKey($"You entered [{s}]");
                        }
                    },
                    new()
                    {
                        Title = "Write color line", Action = () =>
                        {
                            Konsole.WriteLine();
                            Konsole.WriteLine("This is a ♦ggreen ♦wword");
                            Konsole.PressAnyKey();
                            return Task.CompletedTask;
                        }

                    },
                    new()
                    {
                        Title = "Print object as json", Action = () =>
                        {
                            var person = new Person
                            {
                                Age = 42, Name = "Ben Dover", Details = new PersonDetails
                                {
                                    Height = 123,
                                    Description = "undescribed",
                                    DetailedDetails = new DetailedDetails { DetailOne = "Information123", DetailTwo = 123 }
                                }
                            };

                            Konsole.PrintObject(person);
                            
                            Konsole.PressAnyKey();
                            return Task.CompletedTask;
                        }
                    },
                    new()
                    {
                        Title = $"Write to {nameof(KonsoleLogger)}", Action = () =>
                        {
                            Konsole.WriteLine();
                            KonsoleLogger.Log("This is a ♦gcolored message ♦wwhich will appear in " + KonsoleLogger.FilePath + " without escape chars");
                            Konsole.PressAnyKey();
                            return Task.CompletedTask;
                        }

                    }

                }
            };


            if (!Konsole.Confirmed("Are you sure you want to ♦gsee♦= the ♦rsamples♦=?")) return;

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
        public string Description { get; set; }
        public DetailedDetails DetailedDetails { get; set; }
    }

    class DetailedDetails
    {
        public string DetailOne { get; set; }
        public int DetailTwo { get; set; }
    }


}


