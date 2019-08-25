using System;
using PhotoFolder.Core.Domain;
using PhotoFolder.Core.Dto.Services;
using PhotoFolder.Infrastructure.Photos;
using Xunit;

namespace PhotoFolder.Infrastructure.Tests.Photos
{
    public class FilePlaceholderFillerTests
    {
        [Fact]
        public void TestGetPlaceholders()
        {
            var placeholders = new[] {"date:yyyy", "eventName", "hash"};
            var creationDate = DateTimeOffset.Parse("01.07.1999 00:00");
            var fileInfo = new FileInformation("hello.txt", creationDate, creationDate, Hash.Parse("AAFF"), 123,
                creationDate, null, null);

            var result = FilePlaceholderFiller.GetPlaceholders(placeholders, fileInfo);

            Assert.Equal(3, result.Count);
            Assert.Equal("1999", result["date:yyyy"]);
            Assert.Null(result["eventName"]);
            Assert.Equal("aaff", result["hash"]);
        }
    }
}
