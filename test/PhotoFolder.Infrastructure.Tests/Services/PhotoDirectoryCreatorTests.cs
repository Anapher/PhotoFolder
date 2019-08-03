using Moq;
using PhotoFolder.Infrastructure.Consts;
using PhotoFolder.Infrastructure.Photos;
using PhotoFolder.Infrastructure.Serialization;
using PhotoFolder.Infrastructure.Services;
using System;
using System.Collections.Generic;
using System.IO.Abstractions.TestingHelpers;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace PhotoFolder.Infrastructure.Tests.Services
{
    public class PhotoDirectoryCreatorTests
    {
        [Fact]
        public async Task TestCreatePhotoDirectory()
        {
            // arrange
            var fileSystem = new MockFileSystem();
            fileSystem.Directory.CreateDirectory("C:\\test\\folder");

            var mockSerializer = new Mock<IDataSerializer>();
            mockSerializer.Setup(x => x.Serialize(It.IsAny<PhotoDirectoryConfig>())).Returns("Content");

            var service = new PhotoDirectoryCreator(fileSystem, mockSerializer.Object);
            var config = new PhotoDirectoryConfig("test path");
            const string path = "C:\\test\\folder";

            // act
            await service.Create(path, config);

            // assert
            var fileInfo = fileSystem.FileInfo.FromFileName("C:\\test\\folder\\" + PhotoFolderConsts.ConfigFileName);
            Assert.True(fileInfo.Exists);

            var content = fileSystem.File.ReadAllText(fileInfo.FullName);
            Assert.Equal("Content", content);
        }
    }
}
