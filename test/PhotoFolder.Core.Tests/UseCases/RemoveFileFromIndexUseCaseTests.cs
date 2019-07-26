using Moq;
using PhotoFolder.Core.Domain.Entities;
using PhotoFolder.Core.Dto.UseCaseRequests;
using PhotoFolder.Core.Errors;
using PhotoFolder.Core.Interfaces.Gateways;
using PhotoFolder.Core.Interfaces.Gateways.Repositories;
using PhotoFolder.Core.UseCases;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace PhotoFolder.Core.Tests.UseCases
{
    public class RemoveFileFromIndexUseCaseTests
    {
        private static IEqualityComparer<string> GetPathComparer()
        {
            var mockPathComparer = new Mock<IEqualityComparer<string>>();
            mockPathComparer.Setup(x => x.Equals(It.IsAny<string>(), It.IsAny<string>()))
                            .Returns((string s1, string s2) => s1 == s2);

            return mockPathComparer.Object;
        }

        [Fact]
        public async Task CantRemoveFileThatIsNotIndexed()
        {
            // arrange
            var mockDirectory = new Mock<IPhotoDirectory>();
            var mockFileRepository = new Mock<IIndexedFileRepository>();

            mockFileRepository.Setup(x => x.FirstOrDefaultBySpecs(It.IsAny<ISpecification<IndexedFile>[]>())).ReturnsAsync((IndexedFile) null);
            mockDirectory.Setup(x => x.GetFileRepository()).Returns(mockFileRepository.Object);

            var useCase = new RemoveFileFromIndexUseCase();
            var request = new RemoveFileFromIndexRequest("test.xml", mockDirectory.Object);

            // act
            await useCase.Handle(request);

            // assert
            ErrorUtils.AssertError(useCase, ErrorType.InvalidOperation, ErrorCode.FileNotIndexed);
        }

        [Fact]
        public async Task CanRemoveFileAndAlsoRemoveInformation()
        {
            // arrange
            var mockDirectory = new Mock<IPhotoDirectory>();
            var mockFileRepository = new Mock<IIndexedFileRepository>();

            var indexedFile = new IndexedFile(Hash.Parse("FF"), 2312, default, null);
            indexedFile.AddLocation(new FileLocation("test.xml", "FF", default, default));

            mockFileRepository.Setup(x => x.FirstOrDefaultBySpecs(It.IsAny<ISpecification<IndexedFile>[]>())).ReturnsAsync(indexedFile);
            mockDirectory.Setup(x => x.GetFileRepository()).Returns(mockFileRepository.Object);
            mockDirectory.SetupGet(x => x.PathComparer).Returns(GetPathComparer());

            var useCase = new RemoveFileFromIndexUseCase();
            var request = new RemoveFileFromIndexRequest("test.xml", mockDirectory.Object);

            // act
            await useCase.Handle(request);

            // assert
            Assert.False(useCase.HasError);
            mockFileRepository.Verify(x => x.Delete(It.IsAny<IndexedFile>()), Times.Once);
        }

        [Fact]
        public async Task CanRemoveFileButOnlyRemovedIndexedFile()
        {
            // arrange
            var mockDirectory = new Mock<IPhotoDirectory>();
            var mockFileRepository = new Mock<IIndexedFileRepository>();

            var indexedFile = new IndexedFile(Hash.Parse("FF"), 2312, default, null);
            indexedFile.AddLocation(new FileLocation("test.xml", "FF", default, default));
            indexedFile.AddLocation(new FileLocation("test2.xml", "FF", default, default));

            mockFileRepository.Setup(x => x.FirstOrDefaultBySpecs(It.IsAny<ISpecification<IndexedFile>[]>())).ReturnsAsync(indexedFile);
            mockDirectory.Setup(x => x.GetFileRepository()).Returns(mockFileRepository.Object);
            mockDirectory.SetupGet(x => x.PathComparer).Returns(GetPathComparer());

            var useCase = new RemoveFileFromIndexUseCase();
            var request = new RemoveFileFromIndexRequest("test.xml", mockDirectory.Object);

            // act
            await useCase.Handle(request);

            // assert
            Assert.False(useCase.HasError);
            mockFileRepository.Verify(x => x.Update(It.IsAny<IndexedFile>()), Times.Once);
            var keptIndexedFile = Assert.Single(indexedFile.Files);
            Assert.Equal("test2.xml", keptIndexedFile.Filename);
        }
    }
}
