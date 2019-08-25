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

namespace PhotoFolder.Application.IntegrationTests.Workers
{
    public class SynchronizeIndexWorkerTests
    {
        [Fact]
        public Task TestCreateDatabaseAndSynchronize()
        {
            return DefaultPhotoFolder.Initialize(DefaultPhotoFolder.DefaultFileBase);
        }

        [Fact]
        public Task TestCreateDatabaseAndSynchronizeDuplicates()
        {
            return DefaultPhotoFolder.Initialize(new Dictionary<string, string>
            {
                {"egypt_sonyz3.jpg", "egypt_sonyz3.jpg"}, {"asd/asd.jpg", "egypt_sonyz3.jpg"}
            });
        }

        [Fact]
        public async Task TestSynchronizeWithoutChanges()
        {
            var app = await DefaultPhotoFolder.Initialize(DefaultPhotoFolder.DefaultFileBase);

            var loader = app.Container.Resolve<IPhotoDirectoryLoader>();
            var photoDirectory = await loader.Load(DefaultPhotoFolder.PhotoFolderPath);

            var worker = app.Container.Resolve<ISynchronizeIndexWorker>();
            var response = await worker.Execute(new SynchronizeIndexRequest(photoDirectory));

            Assert.Empty(response.Operations);
        }

        [Fact]
        public async Task TestSynchronizeExistingDatabase()
        {
            var app = await DefaultPhotoFolder.Initialize(DefaultPhotoFolder.DefaultFileBase);
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
            var app = await DefaultPhotoFolder.Initialize(DefaultPhotoFolder.DefaultFileBase);
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
