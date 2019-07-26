using PhotoFolder.Infrastructure.Utilities;
using Xunit;

namespace PhotoFolder.Infrastructure.Tests.Utilities
{
    public class PathUtilitiesTests
    {
        [Fact]
        public void TestPatchPathPartsWithoutModifiers()
        {
            // arrange
            var s = "hello/world/file.txt";
            var separators = new[] { '/' };

            // act
            var result = PathUtilities.PatchPathParts(s, separators);

            // assert
            Assert.Equal(s, result);
        }

        [Fact]
        public void TestPatchPathPartsWithDummyModifier()
        {
            // arrange
            var s = "hello/world/file.txt";
            var separators = new[] { '/' };

            // act
            var result = PathUtilities.PatchPathParts(s, separators, x => "wtf");

            // assert
            Assert.Equal("wtf/wtf/wtf", result);
        }

        [Fact]
        public void TestPatchPathPartsWithDifferentSeparators()
        {
            // arrange
            var s = "hello/world\\file.txt";
            var separators = new[] { '/', '\\' };

            // act
            var result = PathUtilities.PatchPathParts(s, separators);

            // assert
            Assert.Equal("hello/world/file.txt", result);
        }

        [Fact]
        public void TestPatchPathPartsWithSinglePart()
        {
            // arrange
            var s = "hello";
            var separators = new[] { '/' };

            // act
            var result = PathUtilities.PatchPathParts(s, separators);

            // assert
            Assert.Equal("hello", result);
        }

        [Fact]
        public void TestTrimChars()
        {
            // arrange
            var s = "  hello - ";

            // act
            var result = PathUtilities.TrimChars(' ', '-')(s);

            // assert
            Assert.Equal("hello", result);
        }

        [Fact]
        public void TestRemoveInvalidChars()
        {
            // arrange
            var s = "abaaabbaab";

            // act
            var result = PathUtilities.RemoveInvalidChars('a')(s);

            // assert
            Assert.Equal("bbbb", result);
        }
    }
}
