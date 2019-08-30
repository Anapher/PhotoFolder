using System.Threading.Tasks;
using Autofac;
using PhotoFolder.Application.Dto.WorkerRequests;
using PhotoFolder.Application.Interfaces.Workers;
using PhotoFolder.Core.Dto.Services.FileIssue;
using PhotoFolder.Infrastructure.Services;
using Xunit;
using System.Linq;
using PhotoFolder.Core.Domain.Entities;
using System;
using System.IO;
using Xunit.Abstractions;

namespace PhotoFolder.Application.IntegrationTests.Workers
{
    public class ImportFilesWorkerTests
    {
        private readonly ITestOutputHelper _output;

        public ImportFilesWorkerTests(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public async Task TestImportNewFile()
        {
            // arrange
            var app = await DefaultPhotoFolder.Initialize(_output, DefaultPhotoFolder.CleanFileBase);

            const string newFilePath = "D:\\Camera\\lando.jpg";
            app.AddResourceFile(newFilePath, "lando_sonyalpha.jpg");

            var loader = app.Container.Resolve<IPhotoDirectoryLoader>();
            var photoDirectory = await loader.Load(DefaultPhotoFolder.PhotoFolderPath);

            var worker = app.Container.Resolve<IImportFilesWorker>();

            // act
            var response = await worker.Execute(new ImportFilesRequest(new[] {newFilePath}, photoDirectory));

            // assert
            var issue = Assert.Single(response.Issues);
            var locationIssue = Assert.IsType<InvalidFileLocationIssue>(issue);

            Assert.Null(locationIssue.File.RelativeFilename);
            Assert.Equal("D:/Camera/lando.jpg", locationIssue.File.Filename);
            Assert.Equal("lando.jpg", locationIssue.CorrectFilename);
            Assert.Equal("2018/01.20", Assert.Single(locationIssue.Suggestions).Directory);
            Assert.Empty(locationIssue.RelevantFiles);
        }

        [Fact]
        public async Task TestDuplicateFile()
        {
            // arrange
            var app = await DefaultPhotoFolder.Initialize(_output, DefaultPhotoFolder.CleanFileBase);

            const string newFilePath = "D:\\Camera\\hanszimmer_htcu11.jpg";
            app.AddResourceFile(newFilePath, "hanszimmer_htcu11.jpg");

            var loader = app.Container.Resolve<IPhotoDirectoryLoader>();
            var photoDirectory = await loader.Load(DefaultPhotoFolder.PhotoFolderPath);

            var worker = app.Container.Resolve<IImportFilesWorker>();

            // act
            var response = await worker.Execute(new ImportFilesRequest(new[] {newFilePath}, photoDirectory));

            // assert
            Assert.Equal(2, response.Issues.Count);
            Assert.Single(response.Issues.OfType<InvalidFileLocationIssue>());

            var duplicateFileIssue = Assert.Single(response.Issues.OfType<DuplicateFilesIssue>());

            Assert.Null(duplicateFileIssue.File.RelativeFilename);
            Assert.Equal("D:/Camera/hanszimmer_htcu11.jpg", duplicateFileIssue.File.Filename);

            var duplicateFile = Assert.Single(duplicateFileIssue.RelevantFiles);
            Assert.Equal("2019/03.30/hanszimmer_htcu11.jpg", duplicateFile.RelativeFilename);
        }

        [Fact]
        public async Task TestSimilarFile()
        {
            // arrange
            var app = await DefaultPhotoFolder.Initialize(_output, DefaultPhotoFolder.CleanFileBase);

            const string newFilePath = "D:\\Camera\\flora.jpg";
            app.AddResourceFile(newFilePath, "flora_sonyalpha_compressed.jpg");

            var loader = app.Container.Resolve<IPhotoDirectoryLoader>();
            var photoDirectory = await loader.Load(DefaultPhotoFolder.PhotoFolderPath);

            var worker = app.Container.Resolve<IImportFilesWorker>();

            // act
            var response = await worker.Execute(new ImportFilesRequest(new[] {newFilePath}, photoDirectory));

            // assert
            Assert.Equal(2, response.Issues.Count);
            Assert.Single(response.Issues.OfType<InvalidFileLocationIssue>());

            var similarFileIssue = Assert.Single(response.Issues.OfType<SimilarFilesIssue>());

            Assert.Null(similarFileIssue.File.RelativeFilename);
            Assert.Equal("D:/Camera/flora.jpg", similarFileIssue.File.Filename);

            var similarFile = Assert.Single(similarFileIssue.RelevantFiles);
            Assert.Equal("2010/05.09/flora_sonyalpha.jpg", similarFile.RelativeFilename);
        }

        [Fact]
        public async Task TestFormerlyDeletedFile()
        {
            // arrange
            var beginTime = DateTimeOffset.UtcNow;
            var app = await DefaultPhotoFolder.Initialize(_output, DefaultPhotoFolder.CleanFileBase);

            var entryToDelete = "2019/03.30/hanszimmer_htcu11.jpg";
            app.MockFileSystem.File.Delete(Path.Combine(DefaultPhotoFolder.PhotoFolderPath, entryToDelete));

            var loader = app.Container.Resolve<IPhotoDirectoryLoader>();
            var photoDirectory = await loader.Load(DefaultPhotoFolder.PhotoFolderPath);

            var synchronizeWorker = app.Container.Resolve<ISynchronizeIndexWorker>();
            var syncResult = await synchronizeWorker.Execute(new SynchronizeIndexRequest(photoDirectory));

            var op = Assert.Single(syncResult.Operations);
            Assert.Equal(FileOperationType.Removed, op.Type);

            // the file should now be stored as a formerly deleted file
            Assert.NotEmpty(photoDirectory.DeletedFiles.Files);

            const string formerlyDeletedFilePath = "C:/hanszimmer.jpg";
            app.AddResourceFile(formerlyDeletedFilePath, "hanszimmer_htcu11.jpg");
            var worker = app.Container.Resolve<IImportFilesWorker>();

            // act
            var response = await worker.Execute(new ImportFilesRequest(new[] { formerlyDeletedFilePath }, photoDirectory));

            // assert
            Assert.Equal(2, response.Issues.Count);
            Assert.Single(response.Issues.OfType<InvalidFileLocationIssue>());

            var formerlyDeletedIssue = Assert.Single(response.Issues.OfType<FormerlyDeletedIssue>());

            Assert.True(formerlyDeletedIssue.DeletedFileInfo.DeletedAt > beginTime, "The deleted at property was set to current time");
            Assert.Equal(entryToDelete, formerlyDeletedIssue.DeletedFileInfo.RelativeFilename);
            Assert.Equal(formerlyDeletedFilePath, formerlyDeletedIssue.File.Filename);
        }
    }
}
