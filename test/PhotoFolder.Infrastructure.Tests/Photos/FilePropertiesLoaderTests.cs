using Moq;
using PhotoFolder.Core.Dto.Services;
using PhotoFolder.Infrastructure.Files;
using System;
using System.IO;
using System.IO.Abstractions.TestingHelpers;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace PhotoFolder.Infrastructure.Tests.Photos
{
    public class FilePropertiesLoaderTests
    {
        [Theory]
        [MemberData(nameof(TestImage.AllTheoryData), MemberType = typeof(TestImage))]
        public async Task TestLoadInformationFromImage(TestImage image)
        {
            var file = new Mock<IFile>();
            file.Setup(x => x.OpenRead()).Returns(() => image.GetStream());

            var loader = new FileInformationLoader(new SHA256FileHasher(), new MockFileSystem());
            var result = await loader.Load(file.Object);

            Assert.Equal(image.Hash.ToString(), result.Hash);
            Assert.NotNull(result.PhotoProperties);

            Assert.Equal(image.Width, result.PhotoProperties.Width);
            Assert.Equal(image.Height, result.PhotoProperties.Height);
            Assert.Equal(image.CreatedOn, result.FileCreatedOn);
        }

        [Fact]
        public async Task TestLoadInformationFromTextFile()
        {
            var textFile = new MemoryStream(Encoding.UTF8.GetBytes("Hello World!"), false);
            var createdOn = new DateTimeOffset(2019, 1, 7, 0, 0, 0, TimeSpan.Zero);

            var file = new Mock<IFile>();
            file.Setup(x => x.OpenRead()).Returns(textFile);
            file.SetupGet(x => x.ModifiedOn).Returns(createdOn);

            var loader = new FileInformationLoader(new SHA256FileHasher(), new MockFileSystem());
            var result = await loader.Load(file.Object);

            Assert.Equal("7f83b1657ff1fc53b92dc18148a1d65dfc2d4b1fa3d677284addd200126d9069", result.Hash);
            Assert.Equal(createdOn, result.FileCreatedOn);
            Assert.Null(result.PhotoProperties);
        }
    }
}
