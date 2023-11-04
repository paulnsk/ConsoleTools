using System.Runtime.CompilerServices;
using System.Linq;
using System;
using System.IO;
using Microsoft.Extensions.Options;


namespace ConsoleTools.KonsoleFileLogger;


internal class KonsoleFileLoggerFilePathProvider
{
    private readonly KonsoleFileLoggerConfig _config;

    public KonsoleFileLoggerFilePathProvider(IOptions<KonsoleFileLoggerConfig> options)
    {
        _config = options.Value;
    }


    public string GetFilePath(string categoryName)
    {
        var dir = GetDir();
        var fileName = GetFileName(categoryName);

        return Path.Combine(dir, fileName);
    }

    private string GetDir()
    {
        var dir = _config.Dir;
        if (dir.IsBlank()) dir = KonsoleFileLoggerConfigConstants.Default;
        return Path.Combine(dir!.SameText(KonsoleFileLoggerConfigConstants.Default) ? Utils.ExeDir() : _config.Dir!, _config.SubDir ?? "");
    }

    private string GetFileName(string categoryName)
    {

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