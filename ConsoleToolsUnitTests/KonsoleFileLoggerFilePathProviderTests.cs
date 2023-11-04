using System.Text.Json.Serialization;
using ConsoleTools;
using ConsoleTools.KonsoleFileLogger;
using Microsoft.Extensions.Options;

namespace ConsoleToolsUnitTests
{
    [TestClass]
    public class KonsoleFileLoggerFilePathProviderTests
    {
        [TestMethod]
        public void GetFilePath__DefaultConfig_ReturnsDefaultFilePath()
        {
            var config = new KonsoleFileLoggerConfig();
            var sut= new KonsoleFileLoggerFilePathProvider(Options.Create(config));
            
            var result = sut.GetFilePath("MyCategory");
            
            Assert.AreEqual(Path.Combine(Utils.ExeDir(), config.SubDir ?? "", Utils.ExeName() + ".k.log"), result);
        }

        [TestMethod]
        public void GetFilePath__DefaultConfigEmptySubdir_ReturnsDefaultFileNameInExeDir()
        {
            var config = new KonsoleFileLoggerConfig { SubDir = null };
            var sut = new KonsoleFileLoggerFilePathProvider(Options.Create(config));

            var result = sut.GetFilePath("MyCategory");
            
            Assert.AreEqual(Path.Combine(Utils.ExeDir(), Utils.ExeName() + ".k.log"), result);
        }

        [TestMethod]
        public void GetFilePath__ConfigWithCategoryNamesNoOverrides_ReturnsCategoryBasedPaths()
        {
            var config = ConfigWithCategoryNames();
            var sut = new KonsoleFileLoggerFilePathProvider(Options.Create(config));
            
            var result = sut.GetFilePath("MyCategory");
            
            Assert.AreEqual(@"C:\MyLogs\Logs\MyCategory.log", result);
        }

        [TestMethod]
        public void GetFilePath__ConfigWithCategorySuffixesNoOverrides_FallsBackToCategoryName()
        {
            var config = ConfigWithCategorySuffixes();
            var sut = new KonsoleFileLoggerFilePathProvider(Options.Create(config));
            
            var result = sut.GetFilePath("MyCategory");
            
            Assert.AreEqual(@"C:\MyLogs\Logs\MyCategory.log", result);
        }


        [TestMethod]
        public void GetFilePath__ConfigWithCategorySuffixOverrides()
        {
            var config = ConfigWithOverridesSuffixes();
            var sut = new KonsoleFileLoggerFilePathProvider(Options.Create(config));
            
            var result = sut.GetFilePath("BaseCategoryAbc");
            Assert.AreEqual(@"C:\MyLogs\Logs\Abc.log", result);


            result = sut.GetFilePath("BaseCategoryXyz");
            Assert.AreEqual(@"C:\MyLogs\Logs\Xyz.log", result);
        }



        [TestMethod]
        public void GetFilePath__ConfigWithCategoryNameOverrides()
        {
            var config = ConfigWithOverridesCategoryNames();
            var sut = new KonsoleFileLoggerFilePathProvider(Options.Create(config));

            var result = sut.GetFilePath("BaseCategoryAbc");
            Assert.AreEqual(@"C:\MyLogs\Logs\BaseCategoryAbc.log", result);


            result = sut.GetFilePath("BaseCategoryXyz");
            Assert.AreEqual(@"C:\MyLogs\Logs\BaseCategoryXyz.log", result);
        }

        [TestMethod]
        public void GetFilePath__ConfigWithCategorySuffixOverridesCategoryWithSubdirs()
        {
            var config = ConfigWithOverridesSuffixes();
            var sut = new KonsoleFileLoggerFilePathProvider(Options.Create(config));

            var result = sut.GetFilePath("BaseCategory\\subdir\\Abc");
            Assert.AreEqual(@"C:\MyLogs\Logs\subdir\Abc.log", result);
        }


        private KonsoleFileLoggerConfig ConfigWithCategoryNames()
        {
            return new KonsoleFileLoggerConfig
            {
                Dir = "C:\\MyLogs",
                FileName = KonsoleFileLoggerConfigConstants.CategoryName
            };
        }

        private KonsoleFileLoggerConfig ConfigWithCategorySuffixes()
        {
            return new KonsoleFileLoggerConfig
            {
                Dir = "C:\\MyLogs",
                FileName = KonsoleFileLoggerConfigConstants.CategorySuffix
            };
        }

        private KonsoleFileLoggerConfig ConfigWithOverridesSuffixes()
        {
            return new KonsoleFileLoggerConfig
            {
                Dir = "C:\\MyLogs",
                CategoryOverrides = new()
                {
                    new CategoryOverrides()
                    {
                        CategoryNameBase = "BaseCategory",
                        FileName = KonsoleFileLoggerConfigConstants.CategorySuffix
                    }
                }
            };
        }


        private KonsoleFileLoggerConfig ConfigWithOverridesCategoryNames()
        {
            return new KonsoleFileLoggerConfig
            {
                Dir = "C:\\MyLogs",
                CategoryOverrides = new()
                {
                    new CategoryOverrides()
                    {
                        CategoryNameBase = "BaseCategory",
                        FileName = KonsoleFileLoggerConfigConstants.CategoryName
                    }
                }
            };
        }

    }
}