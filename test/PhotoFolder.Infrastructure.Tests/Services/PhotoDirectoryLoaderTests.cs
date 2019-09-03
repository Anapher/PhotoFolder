using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Moq;
using PhotoFolder.Infrastructure.Data;
using PhotoFolder.Infrastructure.Photos;
using PhotoFolder.Infrastructure.Serialization;
using PhotoFolder.Infrastructure.Services;
using System.IO;
using System.IO.Abstractions.TestingHelpers;
using System.Threading.Tasks;
using Xunit;

namespace PhotoFolder.Infrastructure.Tests.Services
{
    public class PhotoDirectoryLoaderTests
    {
        [Fact]
        public async Task TestLoadDirectoryWhichDoesNotExist()
        {
            // arrange
            var mockFileSystem = new MockFileSystem();
            var mockSerializer = new Mock<IDataSerializer>();
            var options = Options.Create(new WorkspaceOptions());
            var mockDbContextBuilder = new Mock<IAppDbContextOptionsBuilder>();

            mockSerializer.Setup(x => x.Deserialize<PhotoDirectoryConfig>(It.IsAny<string>()))
                .Returns(new PhotoDirectoryConfig("asd"));

            var loader = new PhotoDirectoryLoader(mockFileSystem, mockSerializer.Object, mockDbContextBuilder.Object,
                options);

            // act/assert
            await Assert.ThrowsAsync<FileNotFoundException>(() => loader.Load("C:\\Pictures"));
        }

        [Fact]
        public async Task TestLoadDirectory()
        {
            // arrange
            var mockFileSystem = new MockFileSystem();
            var mockSerializer = new Mock<IDataSerializer>();
            var options = Options.Create(new WorkspaceOptions());
            var mockDbContextBuilder = new Mock<IAppDbContextOptionsBuilder>();

            mockSerializer.Setup(x => x.Deserialize<PhotoDirectoryConfig>(It.IsAny<string>()))
                .Returns(new PhotoDirectoryConfig("asd"));
            mockFileSystem.AddFile("C:\\Pictures\\.photofolder.json", new MockFileData("test"));

            using (var connection = new SqliteConnection("DataSource=:memory:"))
            {
                connection.Open();

                var builder = new DbContextOptionsBuilder<AppDbContext>();
                builder.UseSqlite(connection);
                mockDbContextBuilder.Setup(x => x.Build(It.IsAny<string>())).Returns(builder.Options);

                var loader = new PhotoDirectoryLoader(mockFileSystem, mockSerializer.Object, mockDbContextBuilder.Object,
                    options);

                // act
                var result = await loader.Load("C:\\Pictures");

                // assert
                Assert.NotNull(result);
            }
        }
    }
}
