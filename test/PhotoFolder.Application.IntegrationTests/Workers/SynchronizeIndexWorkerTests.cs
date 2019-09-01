using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Autofac;
using PhotoFolder.Application.Dto.WorkerRequests;
using PhotoFolder.Application.Interfaces.Workers;
using PhotoFolder.Core.Domain.Entities;
using PhotoFolder.Infrastructure.Services;
using Xunit;
using Xunit.Abstractions;

namespace PhotoFolder.Application.IntegrationTests.Workers
{
    public class SynchronizeIndexWorkerTests
    {
        private readonly ITestOutputHelper _output;

        public SynchronizeIndexWorkerTests(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public async Task TestCreateDatabaseAndSynchronize()
        {
            await using var _ = await DefaultPhotoFolder.Initialize(_output, DefaultPhotoFolder.DefaultFileBase);
        }

        [Fact]
        public async Task TestSynchronizeNonPhotoFile()
        {
            await using var _ = await DefaultPhotoFolder.Initialize(_output, new Dictionary<string, string> {{"2019/23/test.jpg", "textfile.txt"}});
        }

        [Fact]
        public async Task TestNonPhotoFileHasPhotoPropertiesNull()
        {
            await using var app = await DefaultPhotoFolder.Initialize(_output, new Dictionary<string, string> { { "2019/23/test.jpg", "textfile.txt" } });
            var photoFolderPath = DefaultPhotoFolder.PhotoFolderPath;

            var loader = app.Container.Resolve<IPhotoDirectoryLoader>();
            var photoDirectory = await loader.Load(photoFolderPath);

            await using var context = photoDirectory.GetDataContext();

            var files = await context.FileRepository.GetAll();
            var file = files.Single();

            Assert.Null(file.PhotoProperties);
        }

        [Fact]
        public async Task TestAddDuplicateNonPhotoFile()
        {
            await using var app = await DefaultPhotoFolder.Initialize(_output, new Dictionary<string, string> { { "2019/23/test.jpg", "textfile.txt" } });

            var photoFolderPath = DefaultPhotoFolder.PhotoFolderPath;
            app.MockFileSystem.AddFileFromEmbeddedResource(Path.Combine(photoFolderPath, "textfile.jpg"), Assembly.GetExecutingAssembly(),
                "PhotoFolder.Application.IntegrationTests.Resources.textfile.txt");

            var loader = app.Container.Resolve<IPhotoDirectoryLoader>();
            var photoDirectory = await loader.Load(photoFolderPath);

            var worker = app.Container.Resolve<ISynchronizeIndexWorker>();
            var response = await worker.Execute(new SynchronizeIndexRequest(photoDirectory));
        }

        [Fact]
        public async Task TestCreateDatabaseAndSynchronizeDuplicates()
        {
            await using var _ = await DefaultPhotoFolder.Initialize(_output, new Dictionary<string, string>
            {
                {"egypt_sonyz3.jpg", "egypt_sonyz3.jpg"}, {"asd/asd.jpg", "egypt_sonyz3.jpg"}
            });
        }

        [Fact]
        public async Task TestSynchronizeWithoutChanges()
        {
            await using var app = await DefaultPhotoFolder.Initialize(_output, DefaultPhotoFolder.DefaultFileBase);

            var loader = app.Container.Resolve<IPhotoDirectoryLoader>();
            var photoDirectory = await loader.Load(DefaultPhotoFolder.PhotoFolderPath);

            var worker = app.Container.Resolve<ISynchronizeIndexWorker>();
            var response = await worker.Execute(new SynchronizeIndexRequest(photoDirectory));

            Assert.Empty(response.Operations);
        }

        [Fact]
        public async Task TestSynchronizeExistingDatabase()
        {
            await using var app = await DefaultPhotoFolder.Initialize(_output, DefaultPhotoFolder.DefaultFileBase);
            var photoFolderPath = DefaultPhotoFolder.PhotoFolderPath;

            app.MockFileSystem.RemoveFile(Path.Combine(photoFolderPath, "egypt_sonyz3.jpg"));
            app.MockFileSystem.RemoveFile(Path.Combine(photoFolderPath, "flora_sonyalpha.jpg"));
            app.MockFileSystem.AddFileFromEmbeddedResource(Path.Combine(photoFolderPath, "flora_sonyalpha.jpg"), Assembly.GetExecutingAssembly(),
                "PhotoFolder.Application.IntegrationTests.Resources.flora_sonyalpha_compressed.jpg");
            app.MockFileSystem.AddFileFromEmbeddedResource(Path.Combine(photoFolderPath, "lando_sonyalpha.jpg"), Assembly.GetExecutingAssembly(),
                "PhotoFolder.Application.IntegrationTests.Resources.lando_sonyalpha.jpg");

            var loader = app.Container.Resolve<IPhotoDirectoryLoader>();
            var photoDirectory = await loader.Load(photoFolderPath);

            var worker = app.Container.Resolve<ISynchronizeIndexWorker>();
            var response = await worker.Execute(new SynchronizeIndexRequest(photoDirectory));

            Assert.Collection(response.Operations.OrderBy(x => x.TargetFilename), x =>
            {
                Assert.Equal("egypt_sonyz3.jpg", x.TargetFilename);
                Assert.Equal(FileOperationType.Removed, x.Type);
            }, x =>
            {
                Assert.Equal("flora_sonyalpha.jpg", x.TargetFilename);
                Assert.Equal(FileOperationType.Changed, x.Type);
            }, x =>
            {
                Assert.Equal("lando_sonyalpha.jpg", x.TargetFilename);
                Assert.Equal(FileOperationType.New, x.Type);
            });
        }

        [Fact]
        public async Task<ApplicationContext> TestFileMoved()
        {
            await using var app = await DefaultPhotoFolder.Initialize(_output, DefaultPhotoFolder.DefaultFileBase);
            var photoFolderPath = DefaultPhotoFolder.PhotoFolderPath;

            var newDirectory = Path.Combine(photoFolderPath, "2019\\18");
            app.MockFileSystem.Directory.CreateDirectory(newDirectory);
            app.MockFileSystem.File.Move(Path.Combine(photoFolderPath, "egypt_sonyz3.jpg"), Path.Combine(photoFolderPath, newDirectory, "egypt_sonyz3.jpg"));

            var loader = app.Container.Resolve<IPhotoDirectoryLoader>();
            var photoDirectory = await loader.Load(photoFolderPath);

            var worker = app.Container.Resolve<ISynchronizeIndexWorker>();
            var response = await worker.Execute(new SynchronizeIndexRequest(photoDirectory));

            Assert.Collection(response.Operations.OrderBy(x => x.TargetFilename), x =>
            {
                Assert.Equal("egypt_sonyz3.jpg", x.SourceFilename);
                Assert.Equal("2019/18/egypt_sonyz3.jpg", x.TargetFilename);
                Assert.Equal(FileOperationType.Moved, x.Type);
            });

            return app;
        }
    }
}
