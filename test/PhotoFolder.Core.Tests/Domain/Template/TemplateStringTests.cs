using System;
using System.Collections.Generic;
using PhotoFolder.Core.Domain.Template;
using Xunit;

namespace PhotoFolder.Core.Tests.Domain.Template
{
    public class TemplateStringTests
    {
        [Fact]
        public void GivenNoPlaceholders_HasNoPlaceholders_ToString()
        {
            // arrange
            var s = new TemplateString(new[] { new TextFragment("hello"), new TextFragment("world") });

            // act
            var result = s.ToString();

            // assert
            Assert.Equal("helloworld", result);
        }

        [Fact]
        public void GivenNoPlaceholders_HasPlaceholders_ToString()
        {
            // arrange
            var s = new TemplateString(new ITemplateFragment[] { new TextFragment("hello"), new PlaceholderFragment("val"),
                new TextFragment("world") });

            // act
            var result = s.ToString();

            // assert
            Assert.Equal("hello{val}world", result);
        }

        [Fact]
        public void GivenPlaceholders_HasNoPlaceholders_ToString()
        {
            // arrange
            var s = new TemplateString(new ITemplateFragment[] { new TextFragment("hello"), new TextFragment("world") });

            // act
            var result = s.ToString(new Dictionary<string, string> { { "what", "is that" } });

            // assert
            Assert.Equal("helloworld", result);
        }

        [Fact]
        public void GivenPlaceholders_HasPlaceholders_ToString()
        {
            // arrange
            var s = new TemplateString(new ITemplateFragment[] {new PlaceholderFragment("greet"), new TextFragment(" hello"), new PlaceholderFragment("val"),
                new TextFragment("world") });

            // act
            var result = s.ToString(new Dictionary<string, string> { { "val", ", welcome to this " }, { "greet", "bonjour" } });

            // assert
            Assert.Equal("bonjour hello, welcome to this world", result);
        }

        public static readonly TheoryData<ITemplateFragment[], (string, PatternType)[]> TestGeneratePattern =
            new TheoryData<ITemplateFragment[], (string, PatternType)[]>
            {
                // with placeholders
                {
                    new ITemplateFragment[] {new TextFragment("path/to/"), new PlaceholderFragment("eventName"), new TextFragment("/file.txt")},
                    new[] {("^path/to/.*?/file\\.txt$", PatternType.Regex), ("path/to/%/file.txt", PatternType.Like)}
                },
                // without placeholders
                {
                    new ITemplateFragment[] {new TextFragment("path/to/(12.23.2)/"), new TextFragment("file.txt")},
                    new[] {("^path/to/\\(12\\.23\\.2\\)/file\\.txt$", PatternType.Regex), ("path/to/(12.23.2)/file.txt", PatternType.Like)}
                },
                // placeholder after context chars
                {
                    new ITemplateFragment[] {new TextFragment("path/to/34.10 - "), new PlaceholderFragment("file")},
                    new[] {("^path/to/34\\.10.*?$", PatternType.Regex), ("path/to/34.10%", PatternType.Like)}
                },
                // text after context chars
                {
                    new ITemplateFragment[] {new TextFragment("path/to/34.10 - "), new TextFragment("/wtf.txt")},
                    new[] {("^path/to/34\\.10\\ -\\ /wtf\\.txt$", PatternType.Regex)}
                },
                // like operator chars in path
                {
                    new ITemplateFragment[] {new TextFragment("path/to/100%/test_1"), new TextFragment("/wtf.txt")},
                    new[] {(@"path/to/100\%/test\_1/wtf.txt", PatternType.Like)}
                }
            };


        public enum PatternType
        {
            Regex,
            Like
        }

        [Theory]
        [MemberData(nameof(TestGeneratePattern))]
        public void TestProducePatterns(ITemplateFragment[] fragments, (string, PatternType)[] results)
        {
            // arrange
            var s = new TemplateString(fragments);

            // act/assert
            foreach (var (expectedPattern, patternType) in results)
            {
                string actualPattern;
                switch (patternType)
                {
                    case PatternType.Regex:
                        actualPattern = s.ToRegexPattern();
                        break;
                    case PatternType.Like:
                        actualPattern = s.ToLikePattern();
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                Assert.Equal(expectedPattern, actualPattern);
            }
        }
    }
}
