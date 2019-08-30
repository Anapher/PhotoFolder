using Moq;
using PhotoFolder.Core.Dto.Services;
using PhotoFolder.Infrastructure.Files;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Xunit;
using Xunit.Abstractions;

namespace PhotoFolder.Infrastructure.Tests.Photos
{
    public class FilePropertiesLoaderTests
    {
        private readonly ILogger<FileInformationLoader> _logger;

        public FilePropertiesLoaderTests(ITestOutputHelper testOutputHelper)
        {
            var loggerFactory = new LoggerFactory();
            loggerFactory.AddProvider(new XunitLoggerProvider(testOutputHelper));
            _logger = loggerFactory.CreateLogger<FileInformationLoader>();
        }

        [Theory]
        [MemberData(nameof(TestImage.AllTheoryData), MemberType = typeof(TestImage))]
        public async Task TestLoadInformationFromImage(TestImage image)
        {
            var file = new Mock<IFile>();
            file.Setup(x => x.OpenRead()).Returns(() => image.GetStream());

            var loader = new FileInformationLoader(new SHA256FileHasher(), _logger);
            var result = await loader.Load(file.Object);

            Assert.Equal(image.Hash, result.Hash);
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

            var loader = new FileInformationLoader(new SHA256FileHasher(), _logger);
            var result = await loader.Load(file.Object);

            Assert.Equal("7f83b1657ff1fc53b92dc18148a1d65dfc2d4b1fa3d677284addd200126d9069", result.Hash.ToString());
            Assert.Equal(createdOn, result.FileCreatedOn);
            Assert.Null(result.PhotoProperties);
        }
    }
}
