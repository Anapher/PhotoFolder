using Moq;
using PhotoFolder.Application.Workers;
using PhotoFolder.Core;
using PhotoFolder.Core.Domain;
using PhotoFolder.Core.Domain.Entities;
using PhotoFolder.Core.Dto.Services;
using PhotoFolder.Core.Dto.UseCaseRequests;
using PhotoFolder.Core.Dto.UseCaseResponses;
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
        private async Task TestExecute(IEnumerable<IndexedFile> indexedFiles, IEnumerable<IFile> localFiles,
                                      IEnumerable<string> expectedAddedFiles, IEnumerable<string> expectedRemovedFiles,
                                      IEnumerable<FileOperation> expectedOperations, IEnumerable<(long, long)> simliarFiles)
        {
            var removedFiles = new List<string>();
            var addedFiles = new List<string>();
            var operations = new List<FileOperation>();

            // arrange
            var mockServiceProvider = new Mock<IServiceProvider>();
            var mockFileRepository = new Mock<IIndexedFileRepository>();
            var mockPhotoDirectory = new Mock<IPhotoDirectory>();
            var mockFileContentInfoComparer = new Mock<IEqualityComparer<IFileContentInfo>>();
            var mockFileOperationsRepository = new Mock<IFileOperationRepository>();
            var mockPathComparer = new Mock<IEqualityComparer<string>>();

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
                        .Callback((AddFileToIndexRequest x) => addedFiles.Add(x.Filename)).ReturnsAsync((AddFileToIndexRequest request) => {
                            var indexedFile = CreateIndexedFile(localFiles.First(x => x.Filename == request.Filename).Length, request.Filename);
                            return new AddFileToIndexResponse(indexedFile, indexedFile.GetFileByFilename(request.Filename));
                        });

                    return mock.Object;
                });

            mockFileRepository.Setup(x => x.GetAllBySpecs(It.IsAny<IncludeFileLocationsSpec>()))
                .ReturnsAsync(indexedFiles.ToList());
            mockFileOperationsRepository.Setup(x => x.Add(It.IsAny<FileOperation>())).Callback((FileOperation op) => operations.Add(op));
            mockPathComparer.Setup(x => x.Equals(It.IsAny<string>(), It.IsAny<string>())).Returns((string x, string y) => x == y);
            mockPhotoDirectory.Setup(x => x.EnumerateFiles()).Returns(localFiles);
            mockPhotoDirectory.Setup(x => x.GetFileRepository()).Returns(mockFileRepository.Object);
            mockPhotoDirectory.Setup(x => x.GetOperationRepository()).Returns(mockFileOperationsRepository.Object);
            mockPhotoDirectory.SetupGet(x => x.PathComparer).Returns(mockPathComparer.Object);
            mockFileContentInfoComparer.Setup(x => x.Equals(It.IsAny<IFileContentInfo>(), It.IsAny<IFileContentInfo>()))
                .Returns((IFileContentInfo x, IFileContentInfo y) => x.Length == y.Length
                || simliarFiles.Any(z => z.Item1 == x.Length && z.Item2 == y.Length)
                || simliarFiles.Any(z => z.Item1 == y.Length && z.Item2 == x.Length));

            var worker = new SynchronizeIndexWorker(mockServiceProvider.Object, mockFileContentInfoComparer.Object);

            // act
            await worker.Execute(mockPhotoDirectory.Object);

            // assert
            Assert.Equal(expectedAddedFiles.OrderBy(x => x), addedFiles.OrderBy(x => x));
            Assert.Equal(expectedRemovedFiles.OrderBy(x => x), removedFiles.OrderBy(x => x));
            Assert.Collection(operations.OrderBy(x => x.TargetFile.Filename).ThenBy(x => x.Type), expectedOperations.OrderBy(x => x.TargetFile.Filename).ThenBy(x => x.Type).Select(x => new Action<FileOperation>(y => {
                Assert.Equal(x.Type, y.Type);
                Assert.Equal(x.TargetFile.Filename, y.TargetFile.Filename);
                Assert.Equal(x.SourceFile?.Filename, y.SourceFile?.Filename);
            })).ToArray());
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

        private static FileReference GetFileRef(string filename) => new FileReference("FF", filename);

        [Fact]
        public Task TestFileChanged()
        {
            return TestExecute(
                    new[] { CreateIndexedFile(100, "test.xml") },
                    new[] { CreateFile(101, "test.xml") },
                    new[] { "test.xml" },
                    new[] { "test.xml" },
                    new[] {FileOperation.FileChanged(GetFileRef("test.xml"), GetFileRef("test.xml")) },
                    new[] {(100L, 101L)}
                );
        }

        [Fact]
        public Task TestFileReplaced()
        {
            return TestExecute(
                    new[] { CreateIndexedFile(100, "test.xml") },
                    new[] { CreateFile(1, "test.xml") },
                    new[] { "test.xml" },
                    new[] { "test.xml" },
                    new[] { FileOperation.FileRemoved(GetFileRef("test.xml")), FileOperation.NewFile(GetFileRef("test.xml")) },
                    new (long, long)[0]
                );
        }

        [Fact]
        public Task TestFileRemoved()
        {
            return TestExecute(
                    new[] { CreateIndexedFile(100, "test.xml") },
                    new IFile[0],
                    new string[0],
                    new[] { "test.xml" },
                    new[] {FileOperation.FileRemoved(GetFileRef("test.xml"))},
                    new (long, long)[0]
                );
        }

        [Fact]
        public Task TestFileAdded()
        {
            return TestExecute(
                    new IndexedFile[0],
                    new[] { CreateFile(50, "test.xml") },
                    new[] { "test.xml" },
                    new string[0],
                    new[] { FileOperation.NewFile(GetFileRef("test.xml")) },
                    new (long, long)[0]
                );
        }

        [Fact]
        public Task TestMultipleChanges()
        {
            return TestExecute(
                    new[] { CreateIndexedFile(100, "test.xml", "fil2.xml"), CreateIndexedFile(345, "deletedFile.jpg") },
                    new[] { CreateFile(400, "newFile.png"), CreateFile(100, "test.xml"), CreateFile(99, "fil2.xml") },
                    new[] { "fil2.xml", "newFile.png" },
                    new[] { "fil2.xml", "deletedFile.jpg" },
                    new[] { FileOperation.FileChanged(GetFileRef("fil2.xml"), GetFileRef("fil2.xml")), FileOperation.NewFile(GetFileRef("newFile.png")),
                        FileOperation.FileRemoved(GetFileRef("deletedFile.jpg")) },
                    new[] {(100L, 99L)}
                );
        }
    }
}
