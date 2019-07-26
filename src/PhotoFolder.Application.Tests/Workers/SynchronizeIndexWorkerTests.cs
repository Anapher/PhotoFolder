using Moq;
using PhotoFolder.Application.Workers;
using PhotoFolder.Core;
using PhotoFolder.Core.Domain.Entities;
using PhotoFolder.Core.Dto.Services;
using PhotoFolder.Core.Dto.UseCaseRequests;
using PhotoFolder.Core.Interfaces.Gateways;
using PhotoFolder.Core.Interfaces.Gateways.Repositories;
using PhotoFolder.Core.Interfaces.UseCases;
using PhotoFolder.Core.Specifications.FileInformation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace PhotoFolder.Application.Tests.Workers
{
    public class SynchronizeIndexWorkerTests
    {
        public async Task TestExecute(IEnumerable<IndexedFile> indexedFiles, IEnumerable<IFile> localFiles,
                                      IEnumerable<string> expectedAddedFiles, IEnumerable<string> expectedRemovedFiles)
        {
            var removedFiles = new List<string>();
            var addedFiles = new List<string>();

            // arrange
            var mockServiceProvider = new Mock<IServiceProvider>();
            var mockFileRepository = new Mock<IIndexedFileRepository>();
            var mockPhotoDirectory = new Mock<IPhotoDirectory>();

            mockServiceProvider.Setup(x => x.GetService(typeof(IRemoveFileFromIndexUseCase)))
                .Returns(() =>
                {
                    var mock = new Mock<IRemoveFileFromIndexUseCase>();
                    mock.SetupGet(x => x.HasError).Returns(false);
                    mock.Setup(x => x.Handle(It.IsAny<RemoveFileFromIndexRequest>()))
                        .Callback((RemoveFileFromIndexRequest x) => removedFiles.Add(x.Filename));

                    return mock.Object;
                });

            mockServiceProvider.Setup(x => x.GetService(typeof(IAddFileToIndexUseCase)))
                .Returns(() =>
                {
                    var mock = new Mock<IAddFileToIndexUseCase>();
                    mock.SetupGet(x => x.HasError).Returns(false);
                    mock.Setup(x => x.Handle(It.IsAny<AddFileToIndexRequest>()))
                        .Callback((AddFileToIndexRequest x) => addedFiles.Add(x.Filename));

                    return mock.Object;
                });

            mockFileRepository.Setup(x => x.GetAllBySpecs(It.IsAny<IncludeFileLocationsSpec>()))
                .ReturnsAsync(indexedFiles.ToList());
            mockPhotoDirectory.Setup(x => x.EnumerateFiles()).Returns(localFiles);
            mockPhotoDirectory.Setup(x => x.GetFileRepository()).Returns(mockFileRepository.Object);

            var worker = new SynchronizeIndexWorker(mockServiceProvider.Object);

            // act
            await worker.Execute(mockPhotoDirectory.Object);

            // assert
            Assert.Equal(expectedAddedFiles.OrderBy(x => x), addedFiles.OrderBy(x => x));
            Assert.Equal(expectedRemovedFiles.OrderBy(x => x), removedFiles.OrderBy(x => x));
        }

        private static IndexedFile CreateIndexedFile(long length, params string[] filenames)
        {
            var hash = "FF";
            var file = new IndexedFile(Hash.Parse(hash), length, default, default);
            foreach (var filename in filenames)
            {
                file.AddLocation(new FileLocation(filename, hash, default, default));
            }
            return file;
        }

        private static IFile CreateFile(long length, string filename)
        {
            var mock = new Mock<IFile>();
            mock.SetupGet(x => x.Filename).Returns(filename);
            mock.SetupGet(x => x.Length).Returns(length);

            return mock.Object;
        }

        [Fact]
        public Task TestFileChanged()
        {
            return TestExecute(
                    new[] { CreateIndexedFile(100, "test.xml") },
                    new[] { CreateFile(101, "test.xml") },
                    new[] { "test.xml" },
                    new[] { "test.xml" }
                );
        }

        [Fact]
        public Task TestFileRemoved()
        {
            return TestExecute(
                    new[] { CreateIndexedFile(100, "test.xml") },
                    new IFile[0],
                    new string[0],
                    new[] { "test.xml" }
                );
        }

        [Fact]
        public Task TestFileAdded()
        {
            return TestExecute(
                    new IndexedFile[0],
                    new[] { CreateFile(50, "test.xml") },
                    new[] { "test.xml" },
                    new string[0]
                );
        }

        [Fact]
        public Task TestMultipleChanges()
        {
            return TestExecute(
                    new[] { CreateIndexedFile(100, "test.xml", "fil2.xml"), CreateIndexedFile(345, "deletedFile.jpg") },
                    new[] { CreateFile(400, "newFile.png"), CreateFile(100, "test.xml"), CreateFile(99, "fil2.xml") },
                    new[] { "fil2.xml", "newFile.png" },
                    new[] { "fil2.xml", "deletedFile.jpg" }
                );
        }
    }
}
