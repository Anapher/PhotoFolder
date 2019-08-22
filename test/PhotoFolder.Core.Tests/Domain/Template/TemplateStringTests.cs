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

        [Fact]
        public void GivenPlaceholders_ProduceRegex()
        {
            // arrange
            var s = new TemplateString(new ITemplateFragment[] { new TextFragment("path/to/"), new PlaceholderFragment("eventName"),
                new TextFragment("/file.txt")});

            // act
            var result = s.ToRegexPattern();

            // assert
            Assert.Equal("^path/to/.*?/file\\.txt$", result);
        }

        [Fact]
        public void GivenNoPlaceholders_ProduceRegex()
        {
            // arrange
            var s = new TemplateString(new ITemplateFragment[] { new TextFragment("path/to/(12.23.2)/"), new TextFragment("file.txt")});

            // act
            var result = s.ToRegexPattern();

            // assert
            Assert.Equal("^path/to/\\(12\\.23\\.2\\)/file\\.txt$", result);
        }

        [Fact]
        public void HasPlaceholdersAfterContextChars_ProduceRegex()
        {
            // arrange
            var s = new TemplateString(new ITemplateFragment[] { new TextFragment("path/to/34.10 - "), new PlaceholderFragment("file"),  });

            // act
            var result = s.ToRegexPattern();

            // assert
            Assert.Equal("^path/to/34\\.10.*?$", result);
        }

        [Fact]
        public void HasTextAfterContextChars_ProduceRegex()
        {
            // arrange
            var s = new TemplateString(new ITemplateFragment[] { new TextFragment("path/to/34.10 - "), new TextFragment("/wtf.txt"), });

            // act
            var result = s.ToRegexPattern();

            // assert
            Assert.Equal("^path/to/34\\.10\\ -\\ /wtf\\.txt$", result);
        }
    }
}
