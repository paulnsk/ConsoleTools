using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;

namespace ConsoleTools
{
    internal static class Utils
    {
        public static string ExePath()
        {
            return Process.GetCurrentProcess().MainModule?.FileName ?? "";
        }

        public static string ExeDir()
        {
            return Path.GetDirectoryName(ExePath())!;
        }

        public static string ExeName()
        {
            return Path.GetFileName(ExePath());
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

        public static bool SameText(this string @this, string otherString)
        {
            return @this.Equals(otherString, StringComparison.OrdinalIgnoreCase);
        }

        public static bool IsBlank(this string? @this) => string.IsNullOrWhiteSpace(@this);


        public static void EnsureDir(string filePath)
        {
            var dirPath = Path.GetDirectoryName(filePath);
            if (dirPath.IsBlank()) throw new Exception($"{nameof(dirPath)} empty in {nameof(EnsureDir)}");
            if (!Directory.Exists(dirPath)) Directory.CreateDirectory(dirPath!);
            if (!Directory.Exists(dirPath)) throw new Exception($" Unable to create directory {dirPath}");
        }

    }
}
