namespace ConsoleTools
{
    //Full must have same int value  as highest of Messagelevel (e.g. Debug)
    //None must have int value less than lowest of MessageLevel
    //All other values must be same as MessageLevel
    public enum LogLevel
    {
        None = 0,
        Critical = 1,
        Important = 2,
        LessImportant = 3,
        Full = 4
    }



    public enum MessageLevel
    {
        Critical = 1,
        Important = 2,
        LessImportant = 3,
        Debug = 4,
    }



}
