using System;
using System.Reflection;

namespace ConsoleTools
{
    internal static class U
    {
        public static string ExePath()
        {
            var ass = Assembly.GetEntryAssembly();
            if (ass == null) ass = Assembly.GetExecutingAssembly();
            return ass.Location;
        }
        
        /// <summary>
        /// Shows Exception.InnerException recursively
        /// </summary>
        /// <param name="this"></param>
        /// <returns></returns>
        public static string ToStringWithInners(this Exception @this)
        {
            if (@this == null) return "";
            var result = "[" + @this.GetType() + "]: " + @this.Message;
            if (@this.InnerException != null) result += "; Inner: " + @this.InnerException.ToStringWithInners();
            return result;
        }
    }
}
