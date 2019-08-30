using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using PhotoFolder.Application.Dto.WorkerRequests;
using PhotoFolder.Application.Interfaces.Workers;
using PhotoFolder.Core.Domain.Entities;
using PhotoFolder.Core.Dto.Services.FileIssue;
using PhotoFolder.Core.Extensions;
using PhotoFolder.Infrastructure.Services;
using Xunit;
using Xunit.Abstractions;

namespace PhotoFolder.Application.IntegrationTests.Workers
{
    public class CheckFilesWorkerTests
    {
        private readonly ITestOutputHelper _output;

        public CheckFilesWorkerTests(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public async Task TestCheckFiles()
        {
            // arrange
            var app = await DefaultPhotoFolder.Initialize(_output, DefaultPhotoFolder.CleanFileBase);

            var loader = app.Container.Resolve<IPhotoDirectoryLoader>();
            var photoDirectory = await loader.Load(DefaultPhotoFolder.PhotoFolderPath);

            var worker = app.Container.Resolve<ICheckIndexWorker>();

            // act
            var response = await worker.Execute(new CheckIndexRequest(photoDirectory));

            // assert
            Assert.Empty(response.Issues);
        }

        [Fact]
        public async Task TestFileWithInvalidLocation()
        {
            // arrange
            const string file = "hanszimmer_htcu11.jpg";
            const string correctLocation = "2019/03.30/hanszimmer_htcu11.jpg";

            var app = await DefaultPhotoFolder.Initialize(_output, new Dictionary<string, string> {{$"test/{file}", file}});

            var loader = app.Container.Resolve<IPhotoDirectoryLoader>();
            var photoDirectory = await loader.Load(DefaultPhotoFolder.PhotoFolderPath);

            var worker = app.Container.Resolve<ICheckIndexWorker>();

            // act
            var response = await worker.Execute(new CheckIndexRequest(photoDirectory));

            // assert
            var issue = Assert.Single(response.Issues);
            var invalidLocation = Assert.IsType<InvalidFileLocationIssue>(issue);
            Assert.Equal(file, invalidLocation.CorrectFilename);

            var suggestion = Assert.Single(invalidLocation.Suggestions);
            Assert.Equal(correctLocation, suggestion.Filename);
        }

        [Fact]
        public async Task TestFileWithInvalidLocationSuggestExistingFolders()
        {
            // arrange
            const string file = "hanszimmer_htcu11.jpg";
            const string correctFolder = "2019/03.30";
            const string correctLocation = "2019/03.30/hanszimmer_htcu11.jpg";
            var existingFolder = $"{correctFolder} - Hans Zimmer";

            var app = await DefaultPhotoFolder.Initialize(_output, new Dictionary<string, string>
            {
                {$"test/{file}", file},
                { $"{existingFolder}/test.jpg", "egypt_sonyz3.jpg"}
            });

            var loader = app.Container.Resolve<IPhotoDirectoryLoader>();
            var photoDirectory = await loader.Load(DefaultPhotoFolder.PhotoFolderPath);

            var worker = app.Container.Resolve<ICheckIndexWorker>();

            // act
            var response = await worker.Execute(new CheckIndexRequest(photoDirectory));

            // assert
            var issue = Assert.Single(response.Issues.OfType<InvalidFileLocationIssue>().Where(x => x.CorrectFilename == file));

            Assert.Collection(issue.Suggestions.OrderByDescending(x => x.Filename), x => Assert.Equal(correctLocation, x.Filename),
                x => Assert.Equal(existingFolder, x.Directory));
        }

        [Fact]
        public async Task TestFileDuplicate()
        {
            // arrange
            var app = await DefaultPhotoFolder.Initialize(_output, new Dictionary<string, string>
            {
                {"cats/lando/lando.jpg", "lando_sonyalpha.jpg"},
                {"2018/01.20/lando_sonyalpha.jpg", "lando_sonyalpha.jpg"}
            });

            var loader = app.Container.Resolve<IPhotoDirectoryLoader>();
            var photoDirectory = await loader.Load(DefaultPhotoFolder.PhotoFolderPath);

            var worker = app.Container.Resolve<ICheckIndexWorker>();

            // act
            var response = await worker.Execute(new CheckIndexRequest(photoDirectory));

            // assert
            var issue = Assert.Single(response.Issues.OfType<DuplicateFilesIssue>());

            Assert.Single(issue.RelevantFiles);
            Assert.Equal(issue.RelevantFiles.Concat(issue.File.Yield()).Select(x => x.RelativeFilename).OrderBy(x => x),
                new[] {"cats/lando/lando.jpg", "2018/01.20/lando_sonyalpha.jpg"}.OrderBy(x => x));
        }

        [Fact]
        public async Task TestFileSimilar()
        {
            // arrange
            var app = await DefaultPhotoFolder.Initialize(_output, new Dictionary<string, string>
            {
                {"2010/05.09 - Flora Wanderung/flora_sonyalpha_compressed.jpg", "flora_sonyalpha_compressed.jpg"},
                {"2010/05.09 - Flora Wanderung/flora_sonyalpha.jpg", "flora_sonyalpha.jpg"}
            });

            var loader = app.Container.Resolve<IPhotoDirectoryLoader>();
            var photoDirectory = await loader.Load(DefaultPhotoFolder.PhotoFolderPath);

            var worker = app.Container.Resolve<ICheckIndexWorker>();

            // act
            var response = await worker.Execute(new CheckIndexRequest(photoDirectory));

            // assert
            var issue = Assert.Single(response.Issues);
            var similarFileIssue = Assert.IsType<SimilarFilesIssue>(issue);

            Assert.Single(similarFileIssue.RelevantFiles);
            Assert.Equal(similarFileIssue.RelevantFiles.Concat(issue.File.Yield()).Select(x => x.RelativeFilename).OrderBy(x => x),
                new[] {"2010/05.09 - Flora Wanderung/flora_sonyalpha_compressed.jpg", "2010/05.09 - Flora Wanderung/flora_sonyalpha.jpg"}.OrderBy(x => x));
        }

        [Fact]
        public async Task TestFormerlyDeletedFilesAreNotShown()
        {
            // arrange
            var app = await DefaultPhotoFolder.Initialize(_output, DefaultPhotoFolder.CleanFileBase);

            var loader = app.Container.Resolve<IPhotoDirectoryLoader>();
            var photoDirectory = await loader.Load(DefaultPhotoFolder.PhotoFolderPath);

            app.MockFileSystem.File.Delete(Path.Combine(DefaultPhotoFolder.PhotoFolderPath, "2015/10.25/egypt_sonyz3.jpg"));

            var synchronizeWorker = app.Container.Resolve<ISynchronizeIndexWorker>();
            var syncResult = await synchronizeWorker.Execute(new SynchronizeIndexRequest(photoDirectory));

            var op = Assert.Single(syncResult.Operations);
            Assert.Equal(FileOperationType.Removed, op.Type);

            Assert.NotEmpty(photoDirectory.DeletedFiles.Files);

            app.AddResourceFile(Path.Combine(DefaultPhotoFolder.PhotoFolderPath, "test.jpg"), "egypt_sonyz3.jpg");

            synchronizeWorker = app.Container.Resolve<ISynchronizeIndexWorker>();
            syncResult = await synchronizeWorker.Execute(new SynchronizeIndexRequest(photoDirectory));

            op = Assert.Single(syncResult.Operations);
            Assert.Equal(FileOperationType.New, op.Type);

            var worker = app.Container.Resolve<ICheckIndexWorker>();

            // act
            var response = await worker.Execute(new CheckIndexRequest(photoDirectory));

            // assert
            var issue = Assert.Single(response.Issues);
            Assert.IsType<InvalidFileLocationIssue>(issue);
            Assert.Empty(photoDirectory.DeletedFiles.Files);
        }
    }
}
