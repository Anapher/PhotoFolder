using PhotoFolder.Wpf.Services;
using System;
using System.Collections.Generic;
using System.IO.Abstractions.TestingHelpers;
using System.Linq;
using System.Text;
using Xunit;

namespace PhotoFolder.Wpf.Tests.Services
{
    public class AppSettingsProviderTests
    {
        [Fact]
        public void TestNoFileExists()
        {
            // arrange
            var fileSystem = new MockFileSystem();
            var provider = new AppSettingsProvider(fileSystem);

            // act
            var appSettings = provider.Current;

            // assert
            Assert.NotNull(appSettings);
        }

        [Fact]
        public void TestSaveWhenNoFileExists()
        {
            // arrange
            var fileSystem = new MockFileSystem();
            var provider = new AppSettingsProvider(fileSystem);

            // act
            provider.Save(new AppSettings().SetLatestPhotoFolder("Hello World"));

            // assert
            Assert.Equal("Hello World", provider.Current.LatestPhotoFolder);
            Assert.Single(fileSystem.AllFiles);
        }

        [Fact]
        public void TestLoadExistingFile()
        {
            // arrange
            var fileSystem = new MockFileSystem();
            var provider = new AppSettingsProvider(fileSystem);
            provider.Save(new AppSettings().SetLatestPhotoFolder("hello world"));
            provider = new AppSettingsProvider(fileSystem);

            // act/assert
            Assert.Equal("hello world", provider.Current.LatestPhotoFolder);
        }
    }
}
