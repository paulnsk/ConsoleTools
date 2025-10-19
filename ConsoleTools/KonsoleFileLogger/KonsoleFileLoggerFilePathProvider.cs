using System.Runtime.CompilerServices;
using System.Linq;
using System;
using System.IO;
using Microsoft.Extensions.Options;


namespace ConsoleTools.KonsoleFileLogger;

/// <summary>
/// Resolves the full log file path for a given logger category.
/// It uses the 'KonsoleFileLoggerConfig' settings to combine a base directory,
/// a subdirectory, and a file name, which can be overridden for specific category prefixes.
/// </summary>
internal class KonsoleFileLoggerFilePathProvider(IOptions<KonsoleFileLoggerConfig> options)
{
    // todo clean this up...
    /*
        Configuration Cheatsheet: How the Log File Path is Determined
        --------------------------------------------------------------

        The final path is constructed as: [Resolved Directory] + [Resolved File Name].

        1. DIRECTORY RESOLUTION (`Dir` and `SubDir` properties)
           - The base directory is determined by the `Dir` property:
             - If `Dir` is "Default", null, or empty: Uses the application's startup directory.
             - Otherwise: Uses the literal path specified in `Dir`.
           - The `SubDir` value is always appended to the base directory.
           - Examples:
             - { "Dir": "C:\\Logs", "SubDir": "MyApp" } -> C:\Logs\MyApp\
             - { "Dir": "Default", "SubDir": "Logs" }   -> [AppDir]\Logs\

        2. FILE NAME RESOLUTION (`FileName` and `CategoryOverrides` properties)
           This is a two-step process: First find the rule, then interpret it.

           Step A: Find the Governing `FileName` Rule
           - The system checks if the logger's category matches any `CategoryOverrides`.
           - It uses the `FileName` rule from the *first* override where the logger's
             category starts with the `CategoryNameBase` of the override.
           - If no override matches, the global `FileName` rule from the root config is used.

           Step B: Interpret the `FileName` Rule String
           The string obtained from Step A is resolved as follows:

           - If the string is a special constant:
             - "Default":        Uses the executable name. (e.g., "MyApi.exe.k.log")
             - "CategoryName":   Uses the logger's full category name. (e.g., "MyApp.Services.UserService.log")
             - "CategorySuffix": Uses the part of the category name that comes *after* the
                               `CategoryNameBase` from the matching override.
                               (e.g., category "api.controllers.users" + base "api.controllers." -> "users.log")

           - If the string is anything else:
             - It is used as a literal file name. (e.g., "_http_client.log")

        --- Concrete Example ---

        Given the config:
        {
          "FileName": "_default.log",
          "CategoryOverrides": [
            { "CategoryNameBase": "MyApp.Services.", "FileName": "CategorySuffix" },
            { "CategoryNameBase": "Microsoft.", "FileName": "_ms.log" }
          ]
        }

        The following category names would resolve to these file names:
        - "MyApp.Services.UserService" -> "UserService.log"
        - "Microsoft.AspNetCore.Hosting" -> "_ms.log"
        - "Some.Other.Namespace" -> "_default.log"
    */

    private readonly KonsoleFileLoggerConfig _config = options.Value;


    public string GetFilePath(string categoryName)
    {
        var dir = GetDir();
        var fileName = GetFileName(categoryName);

        return Path.Combine(dir, fileName);
    }

    private string GetDir()
    {
        if (_config.DisableFile) return string.Empty;
        var dir = _config.Dir;
        if (dir.IsBlank()) dir = KonsoleFileLoggerConfigConstants.Default;
        return Path.Combine(dir!.SameText(KonsoleFileLoggerConfigConstants.Default) ? Utils.ExeDir() : _config.Dir!, _config.SubDir ?? "");
    }

    private string GetFileName(string categoryName)
    {
        if (_config.DisableFile) return string.Empty;

        var fileSpec = _config.FileName;
        var matchingOverride = _config.CategoryOverrides.FirstOrDefault(x => !Utils.IsBlank(x.CategoryNameBase) && categoryName.StartsWith(x.CategoryNameBase!, StringComparison.InvariantCultureIgnoreCase));
        if (matchingOverride != null)
        {
            fileSpec = matchingOverride.FileName;
        }

        if (fileSpec.IsBlank()) fileSpec = KonsoleFileLoggerConfigConstants.Default;
        if (fileSpec!.SameText(KonsoleFileLoggerConfigConstants.Default))
        {
            return Utils.ExeName() + ".k.log";
        }

        if (fileSpec!.SameText(KonsoleFileLoggerConfigConstants.CategoryName))
        {
            return categoryName + ".log";
        }

        if (fileSpec!.SameText(KonsoleFileLoggerConfigConstants.CategorySuffix))
        {
            var categoryNameBase = matchingOverride?.CategoryNameBase ?? "";
            if (categoryNameBase.IsBlank())
            {
                return categoryName + ".log";
            } 
            var suffix = categoryName.Replace(categoryNameBase, "");
            if (suffix.IsBlank()) suffix = categoryName;
            if (suffix.StartsWith("\\")) suffix = suffix.Remove(0, 1);
            return suffix + ".log";
        }

        return fileSpec;

    }

}