// ReSharper disable CollectionNeverUpdated.Global
// ReSharper disable ClassNeverInstantiated.Global
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ConsoleTools.KonsoleFileLogger
{
    public static class KonsoleFileLoggerConfigConstants
    {
        public const string Default = nameof(Default);
        public const string CategoryName = nameof(CategoryName);
        public const string CategorySuffix = nameof(CategorySuffix);
    }

    public class KonsoleFileLoggerConfig
    {
        public string? Dir { get; set; } = default;
        public string? SubDir { get; set; } = "Logs";

        /// <summary>
        /// file name including optional subdir(s) e.g. data\logs\log.txt, or one of <see cref="KonsoleFileLoggerConfigConstants"/>
        /// </summary>
        public string FileName { get; set; } = KonsoleFileLoggerConfigConstants.Default;
        
        public List<string> SuppressedCategories { get; set; } = new();
        
        public List<CategoryOverrides> CategoryOverrides { get; set; } = new();
    }

    public class CategoryOverrides
    {
        [MinLength(1)]
        public string? CategoryNameBase { get; set; }
        public string FileName { get; set; } = KonsoleFileLoggerConfigConstants.CategoryName;
    }
    

}


