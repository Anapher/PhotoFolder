using Moq;
using PhotoFolder.Core.Domain.Entities;
using PhotoFolder.Core.Dto.UseCaseRequests;
using PhotoFolder.Core.Dto.UseCaseResponses;
using PhotoFolder.Core.Interfaces.Gateways;
using PhotoFolder.Core.Interfaces.Services;
using PhotoFolder.Core.UseCases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace PhotoFolder.Core.Tests.UseCases
{
    public class CheckFileIntegrityUseCaseTests
    {
        private static FileInformation CreateFileInformation(string filename, string fileHash, string bitmapHash = null)
        {
            var photoProperties = bitmapHash == null ? null : new Core.Domain.PhotoProperties(Hash.Parse(bitmapHash), default, default);
            return new FileInformation(filename, default, default, Hash.Parse(fileHash), default, default, photoProperties);
        }

        private static IndexedFile CreateIndexedFile(string fileHash, string bitmapHash, params string[] filenames)
        {
            var photoProperties = bitmapHash == null ? null : new Core.Domain.PhotoProperties(Hash.Parse(bitmapHash), default, default);
            var file = new IndexedFile(Hash.Parse(fileHash), default, default, photoProperties);
            foreach (var filename in filenames)
            {
                file.AddLocation(new FileLocation(filename, fileHash, default, default));
            }
            return file;
        }

        private async Task TestHandle(FileInformation file, IEnumerable<IndexedFile> indexedFiles, string directoryRegex, string filenameRegex, CheckFileIntegrityResponse expectedResponse)
        {
            // arrange
            var mockBitmapHashComparer = new Mock<IBitmapHashComparer>();
            var mockPhotoDirectory = new Mock<IPhotoDirectory>();
            var mockPathComparer = new Mock<IEqualityComparer<string>>();

            mockPathComparer.Setup(x => x.Equals(It.IsAny<string>(), It.IsAny<string>())).Returns((string x, string y) => x == y);
            mockBitmapHashComparer.Setup(x => x.Compare(It.IsAny<Hash>(), It.IsAny<Hash>())).Returns((Hash x, Hash y) => x.Equals(y) ? 1F : 0F);
            mockPhotoDirectory.SetupGet(x => x.PathComparer).Returns(mockPathComparer.Object);
            mockPhotoDirectory.Setup(x => x.GetFileDirectoryRegexPattern(file)).Returns(directoryRegex);
            mockPhotoDirectory.Setup(x => x.GetFilenameRegexPattern(file)).Returns(filenameRegex);

            var useCase = new CheckFileIntegrityUseCase(mockBitmapHashComparer.Object);
            var request = new CheckFileIntegrityRequest(file, indexedFiles, mockPhotoDirectory.Object);

            // handle
            var response = await useCase.Handle(request);

            // assert
            Assert.False(useCase.HasError);
            Assert.Collection(response.EqualFiles.OrderBy(x => x.Filename), expectedResponse.EqualFiles.OrderBy(x => x.Filename).Select(x => new Action<FileLocation>(y => {
                Assert.Equal(x.Filename, y.Filename);
            })).ToArray());
            Assert.Collection(response.SimilarFiles.OrderBy(x => x.Key.Hash), expectedResponse.SimilarFiles.OrderBy(x => x.Key.Hash).Select(x => new Action<KeyValuePair<IndexedFile, float>>(y => {
                Assert.Equal(x.Value, y.Value);
                Assert.Equal(x.Key.Hash, y.Key.Hash);
            })).ToArray());
            Assert.Equal(expectedResponse.IsWrongPlaced, response.IsWrongPlaced);
            Assert.Equal(expectedResponse.RecommendedFilename, response.RecommendedFilename);
            Assert.Equal(expectedResponse.RecommendedDirectories?.OrderBy(x => x), response.RecommendedDirectories?.OrderBy(x => x));
        }

        [Fact]
        public Task TestCheckFileThatIsNewAndIsNoBitmap_NoIndexedFiles()
        {
            return TestHandle(CreateFileInformation(@"C:\hello\test.xml", "FF"), new IndexedFile[0], @"^2019\04.23$", "^.+?$",
                new CheckFileIntegrityResponse(new List<FileLocation>(), new Dictionary<IndexedFile, float>(), true, new List<string>(), null));
        }

        [Fact]
        public Task TestCheckFileThatIsNewAndIsBitmap_NoIndexedFiles()
        {
            return TestHandle(CreateFileInformation(@"C:\hello\test.xml", "FF", "AA"), new IndexedFile[0], @"^2019\04.23$", "^.+?$",
                new CheckFileIntegrityResponse(new List<FileLocation>(), new Dictionary<IndexedFile, float>(), true, new List<string>(), null));
        }

        [Fact]
        public Task TestCheckFileThatIsNewAndIsBitmap_FileWithTargetDirectoryExists()
        {
            return TestHandle(CreateFileInformation(@"C:\hello\test.xml", "FF", "AA"), new List<IndexedFile> { CreateIndexedFile("EE", null,  @"2019\04.23 - MilPat\test.png") }, @"^2019\\04\.23 .+?$", "^.+?$",
                new CheckFileIntegrityResponse(new List<FileLocation>(), new Dictionary<IndexedFile, float>(), true, new List<string> { @"2019\04.23 - MilPat" }, null));
        }

        [Fact]
        public Task TestCheckFileThatIsNewAndIsBitmap_SimliarFileWithTargetDirectoryExists()
        {
            var simliarFile = CreateIndexedFile("EE", "AA", @"2019\04.23 - MilPat\test.png");
            return TestHandle(CreateFileInformation(@"C:\hello\test.xml", "FF", "AA"), new List<IndexedFile> { simliarFile }, @"^2019\\04\.23 .+?$", "^.+?$",
                new CheckFileIntegrityResponse(new List<FileLocation>(), new Dictionary<IndexedFile, float> { { simliarFile, 1F } }, true, new List<string> { @"2019\04.23 - MilPat" }, null));
        }

        [Fact]
        public Task TestCheckFileThatIsNewAndIsBitmap_EqualFileWithTargetDirectoryExists()
        {
            var equalFile = CreateIndexedFile("EE", "AD", @"2019\04.23 - MilPat\test.png");
            return TestHandle(CreateFileInformation(@"C:\hello\test.xml", "EE", "AA"), new List<IndexedFile> { equalFile }, @"^2019\\04\.23 .+?$", "^.+?$",
                new CheckFileIntegrityResponse(new List<FileLocation> { new FileLocation(@"2019\04.23 - MilPat\test.png", "EE", default, default) }, new Dictionary<IndexedFile, float>(), true, new List<string> { @"2019\04.23 - MilPat" }, null));
        }
    }
}
