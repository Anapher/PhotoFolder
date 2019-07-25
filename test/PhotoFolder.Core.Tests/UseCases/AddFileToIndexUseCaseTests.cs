using Moq;
using PhotoFolder.Core.Domain.Entities;
using PhotoFolder.Core.Dto.Services;
using PhotoFolder.Core.Dto.UseCaseRequests;
using PhotoFolder.Core.Errors;
using PhotoFolder.Core.Interfaces.Gateways;
using PhotoFolder.Core.Interfaces.Gateways.Repositories;
using PhotoFolder.Core.Interfaces.Services;
using PhotoFolder.Core.Specifications;
using PhotoFolder.Core.Specifications.FileInformation;
using PhotoFolder.Core.UseCases;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace PhotoFolder.Core.Tests.UseCases
{
    public class AddFileToIndexUseCaseTests
    {
        private readonly IFileHasher _fileHasher;
        private static readonly Hash _fileHash = Hash.Parse("ABBA");
        private static IndexedFile _testFileInformation =>
            new IndexedFile(_fileHash, 5, default, default);

        public AddFileToIndexUseCaseTests()
        {
            var mockFileHasher = new Mock<IFileHasher>();
            mockFileHasher.Setup(x => x.ComputeHash(It.IsAny<Stream>())).Returns(_fileHash);
            _fileHasher = mockFileHasher.Object;
        }

        private static IEqualityComparer<IFileContentInfo> GetFileComparer(bool equals = true)
        {
            var mock = new Mock<IEqualityComparer<IFileContentInfo>>();
            mock.Setup(x => x.Equals(It.IsAny<IFileContentInfo>(), It.IsAny<IFileContentInfo>()))
                .Returns(equals);

            return mock.Object;
        }

        private static Mock<IFileInformationLoader> GetFileLoader(FileInformation returnValue = null)
        {
            if (returnValue == null)
                returnValue = new FileInformation("test.xml", default, default, "ABBA", 324, default, null);

            var mock = new Mock<IFileInformationLoader>();
            mock.Setup(x => x.Load(It.IsAny<IFile>())).ReturnsAsync(returnValue);

            return mock;
        }

        private static Mock<IIndexedFileRepository> GetFileRepository(IndexedFile fileWithSameFilename = null,
            IndexedFile fileWithSameHash = null)
        {
            var mockFileRepository = new Mock<IIndexedFileRepository>();

            mockFileRepository
                .Setup(x => x.FirstOrDefaultBySpecs(It.IsAny<FindByFilenameSpec>()))
                .ReturnsAsync(fileWithSameFilename);

            mockFileRepository
                .Setup(x => x.FirstOrDefaultBySpecs(It.IsAny<FindByFileHashSpec>(),
                    It.IsAny<IncludeFileLocationsSpec>()))
                .ReturnsAsync(fileWithSameHash);

            return mockFileRepository;
        }

        [Fact]
        public async Task CantAddNonExistingFile()
        {
            // arrange
            var mockDirectory = new Mock<IPhotoDirectory>();
            mockDirectory.Setup(x => x.GetFile(It.IsAny<string>())).Returns((IFile) null);

            var useCase = new AddFileToIndexUseCase(GetFileLoader().Object, _fileHasher, GetFileComparer());
            var request = new AddFileToIndexRequest("test.jpg", mockDirectory.Object);

            // act
            await useCase.Handle(request);

            // assert
            ErrorUtils.AssertError(useCase, ErrorType.InvalidOperation, ErrorCode.FileNotFound);
        }

        [Fact]
        public async Task CantAddFileThatIsAlreadyIndexed()
        {
            // arrange
            var mockDirectory = new Mock<IPhotoDirectory>();
            var mockFile = new Mock<IFile>();
            var mockFileRepository = GetFileRepository(_testFileInformation);

            mockFile.SetupGet(x => x.Filename).Returns("test.jpg");
            mockDirectory.Setup(x => x.GetFile(It.IsAny<string>())).Returns(mockFile.Object);
            mockDirectory.Setup(x => x.GetFileRepository()).Returns(mockFileRepository.Object);

            var useCase = new AddFileToIndexUseCase(GetFileLoader().Object, _fileHasher, GetFileComparer());
            var request = new AddFileToIndexRequest("test.xml", mockDirectory.Object);

            // act
            await useCase.Handle(request);

            // assert
            ErrorUtils.AssertError(useCase, ErrorType.InvalidOperation, ErrorCode.FileAlreadyIndexed);
        }

        [Fact]
        public async Task CantAddFileThatWasRemoved()
        {
            // arrange
            var mockDirectory = new Mock<IPhotoDirectory>();
            var mockFile = new Mock<IFile>();
            var mockFileRepository = GetFileRepository();
            var mockFileInformationLoader = GetFileLoader();

            var calls = 0;

            mockFile.SetupGet(x => x.Filename).Returns("test.jpg");
            mockDirectory.Setup(x => x.GetFile(It.IsAny<string>())).Returns(() => calls++ == 0 ? mockFile.Object : null);
            mockDirectory.Setup(x => x.GetFileRepository()).Returns(mockFileRepository.Object);
            mockFileInformationLoader.Setup(x => x.Load(It.IsAny<IFile>()))
                .ThrowsAsync(new FileNotFoundException());

            var useCase = new AddFileToIndexUseCase(mockFileInformationLoader.Object, _fileHasher, GetFileComparer());
            var request = new AddFileToIndexRequest("test.xml", mockDirectory.Object);

            // act
            await useCase.Handle(request);

            // assert
            ErrorUtils.AssertError(useCase, ErrorType.InvalidOperation, ErrorCode.FileNotFound);
        }

        [Fact]
        public async Task CanAddNewFile()
        {
            // arrange
            var mockDirectory = new Mock<IPhotoDirectory>();
            var mockFile = new Mock<IFile>();
            var mockFileRepository = GetFileRepository();
            var mockOperationsRepository = new Mock<IFileOperationRepository>();

            mockFile.SetupGet(x => x.Filename).Returns("test.jpg");
            mockDirectory.Setup(x => x.GetFile(It.IsAny<string>())).Returns(mockFile.Object);
            mockDirectory.Setup(x => x.GetFileRepository()).Returns(mockFileRepository.Object);
            mockDirectory.Setup(x => x.GetOperationRepository()).Returns(mockOperationsRepository.Object);

            var useCase = new AddFileToIndexUseCase(GetFileLoader().Object, _fileHasher, GetFileComparer());
            var request = new AddFileToIndexRequest("test.xml", mockDirectory.Object);

            // act
            await useCase.Handle(request);

            // assert
            Assert.False(useCase.HasError);
            mockOperationsRepository.Verify(
                x => x.Add(It.Is<FileOperation>(y => y.Type == FileOperationType.New)),
                Times.Once);
            mockFileRepository.Verify(x => x.Add(It.Is<IndexedFile>(y => y.Files.Count() == 1)), Times.Once);
        }

        [Fact]
        public async Task CanAddNewFileThatHasFileInformationStoredInRepo()
        {
            // arrange
            var mockDirectory = new Mock<IPhotoDirectory>();
            var mockFile = new Mock<IFile>();
            var mockFileRepository = GetFileRepository(null, _testFileInformation);
            var mockOperationsRepository = new Mock<IFileOperationRepository>();
            var mockFileLoader = GetFileLoader();

            mockFile.SetupGet(x => x.Filename).Returns("test.jpg");
            mockDirectory.Setup(x => x.GetFile(It.IsAny<string>())).Returns(mockFile.Object);
            mockDirectory.Setup(x => x.GetFileRepository()).Returns(mockFileRepository.Object);
            mockDirectory.Setup(x => x.GetOperationRepository()).Returns(mockOperationsRepository.Object);

            var useCase = new AddFileToIndexUseCase(mockFileLoader.Object, _fileHasher, GetFileComparer());
            var request = new AddFileToIndexRequest("test.xml", mockDirectory.Object);

            // act
            await useCase.Handle(request);

            // assert
            Assert.False(useCase.HasError);
            mockOperationsRepository.Verify(
                x => x.Add(It.Is<FileOperation>(y => y.Type == FileOperationType.New)),
                Times.Once);
            mockFileRepository.Verify(x => x.Update(It.Is<IndexedFile>(y => y.Files.Count() == 1)), Times.Once);
            mockFileLoader.Verify(x => x.Load(It.IsAny<IFile>()), Times.Never);
        }

        [Fact]
        public async Task CanDetectMoveFile()
        {
            // arrange
            var mockDirectory = new Mock<IPhotoDirectory>();
            var mockFile = new Mock<IFile>();
            var mockFileRepository = GetFileRepository();
            var mockOperationsRepository = new Mock<IFileOperationRepository>();
            var mockPathComparer = new Mock<IEqualityComparer<string>>();

            mockFile.SetupGet(x => x.Filename).Returns("test.jpg");
            mockPathComparer.Setup(x => x.Equals(It.IsAny<string>(), It.IsAny<string>())).Returns(false);
            mockDirectory.Setup(x => x.GetFile(It.IsAny<string>())).Returns(mockFile.Object);
            mockDirectory.Setup(x => x.GetFileRepository()).Returns(mockFileRepository.Object);
            mockDirectory.Setup(x => x.GetOperationRepository()).Returns(mockOperationsRepository.Object);
            mockDirectory.SetupGet(x => x.PathComparer).Returns(mockPathComparer.Object);

            var file = new FileInformation("oldtest.xml", default, default, "AAFF", 343, default, null);

            var removedFiles = new List<FileInformation> { file };
            var useCase = new AddFileToIndexUseCase(GetFileLoader().Object, _fileHasher, GetFileComparer());
            var request = new AddFileToIndexRequest("test.xml", mockDirectory.Object, removedFiles.ToImmutableList());

            // act
            await useCase.Handle(request);

            // assert
            Assert.False(useCase.HasError);
            mockOperationsRepository.Verify(
                x => x.Add(It.Is<FileOperation>(y => y.Type == FileOperationType.Moved)),
                Times.Once);
            mockFileRepository.Verify(x => x.Add(It.Is<IndexedFile>(y => y.Files.Count() == 1)), Times.Once);
        }

        [Fact]
        public async Task CanDetectChangedFile()
        {
            // arrange
            var mockDirectory = new Mock<IPhotoDirectory>();
            var mockFile = new Mock<IFile>();
            var mockFileRepository = GetFileRepository();
            var mockOperationsRepository = new Mock<IFileOperationRepository>();
            var mockPathComparer = new Mock<IEqualityComparer<string>>();

            mockFile.SetupGet(x => x.Filename).Returns("test.jpg");
            mockPathComparer.Setup(x => x.Equals(It.IsAny<string>(), It.IsAny<string>())).Returns(true);
            mockDirectory.Setup(x => x.GetFile(It.IsAny<string>())).Returns(mockFile.Object);
            mockDirectory.Setup(x => x.GetFileRepository()).Returns(mockFileRepository.Object);
            mockDirectory.Setup(x => x.GetOperationRepository()).Returns(mockOperationsRepository.Object);
            mockDirectory.SetupGet(x => x.PathComparer).Returns(mockPathComparer.Object);

            var file = new FileInformation("test.xml", default, default, "AAFF", 343, default, null);
            var removedFiles = new List<FileInformation> { file };
            var useCase = new AddFileToIndexUseCase(GetFileLoader().Object, _fileHasher, GetFileComparer());
            var request = new AddFileToIndexRequest("test.xml", mockDirectory.Object, removedFiles.ToImmutableList());

            // act
            await useCase.Handle(request);

            // assert
            Assert.False(useCase.HasError);
            mockOperationsRepository.Verify(
                x => x.Add(It.Is<FileOperation>(y => y.Type == FileOperationType.Changed)),
                Times.Once);
            mockFileRepository.Verify(x => x.Add(It.Is<IndexedFile>(y => y.Files.Count() == 1)), Times.Once);
        }
    }
}
