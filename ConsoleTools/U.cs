using System;
using System.Reflection;

namespace ConsoleTools
{
    //todo remove, use Utils
    internal static class U
    {
        public static string ExePath()
        {
            var ass = Assembly.GetEntryAssembly();
            if (ass == null) ass = Assembly.GetExecutingAssembly();
            return ass.Location;
        }

        public static int ToInt(this string @this)
        {
            var result = -1;
            try
            {
                result = Convert.ToInt32(@this);
            }

            catch
            {
            }

            return result;
        }

    }
}
