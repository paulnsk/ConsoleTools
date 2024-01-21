using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using Microsoft.Extensions.Options;

namespace ConsoleTools.KonsoleFileLogger;

internal class KonsoleFileLoggerProvider : ILoggerProvider
{
    private readonly KonsoleFileLoggerFilePathProvider _filePathProvider;
    private readonly KonsoleFileLoggerConfig _config;

    public KonsoleFileLoggerProvider(IOptions<KonsoleFileLoggerConfig> options, KonsoleFileLoggerFilePathProvider filePathProvider)
    {
        _filePathProvider = filePathProvider;
        _config = options.Value;
    }
    

    public ILogger CreateLogger(string categoryName)
    {
        var filePath = _filePathProvider.GetFilePath(categoryName);
        Utils.EnsureDir(filePath);
        return new KonsoleFileLogger(categoryName, filePath, _config);
    }

    public void Dispose()
    {
    }

     
}