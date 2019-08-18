using PhotoFolder.Core.Domain.Entities;
using System;
using PhotoFolder.Core.Domain;
using Xunit;

namespace PhotoFolder.Core.Tests.Domain.Entities
{
    public class IndexedFileTests
    {
        [Fact]
        public void HasNoFiles_RemoveFile_ShouldThrow()
        {
            // arrange
            var indexedFile = new IndexedFile(Hash.Parse("FF"), 1, DateTimeOffset.MinValue, null);

            // act/assert
            Assert.Throws<ArgumentException>(() => indexedFile.RemoveLocation("test.txt"));
        }

        [Fact]
        public void HasOneFiles_RemoveFile_ShouldRemove()
        {
            // arrange
            var indexedFile = new IndexedFile(Hash.Parse("FF"), 1, default, null);
            indexedFile.AddLocation(new FileLocation("test.txt", "FF", default, default));

            // act
            indexedFile.RemoveLocation("test.txt");

            // assert
            Assert.Empty(indexedFile.Files);
        }

        [Fact]
        public void HasNoFiles_AddFile_ShouldAdd()
        {
            // arrange
            var indexedFile = new IndexedFile(Hash.Parse("FF"), 1, default, null);

            // act
            indexedFile.AddLocation(new FileLocation("test.txt", "FF", default, default));

            // assert
            Assert.NotEmpty(indexedFile.Files);
        }

        [Fact]
        public void HasNoFiles_AddSameFile_ShouldThrow()
        {
            // arrange
            var fileInformation = new IndexedFile(Hash.Parse("FF"), 1, default, null);

            var fileLocation = new FileLocation("test.txt", "FF", default, default);
            fileInformation.AddLocation(fileLocation);

            // act/assert
            Assert.Throws<ArgumentException>(() => fileInformation.AddLocation(fileLocation));

            // assert
            Assert.Single(fileInformation.Files);
        }
    }
}
