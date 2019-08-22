using System;
using System.Collections.Generic;
using System.Linq;
using PhotoFolder.Core.Domain.Template;
using PhotoFolder.Core.Services;
using Xunit;

namespace PhotoFolder.Core.Tests.Domain.Template
{
    public class TemplateStringParserTests
    {
        public static readonly TheoryData<string, IEnumerable<ITemplateFragment>> TestData
            = new TheoryData<string, IEnumerable<ITemplateFragment>>
            {
                {"hello world", new [] { new TextFragment("hello world")} },
                {"{one}{two}", new [] { new PlaceholderFragment("one"), new PlaceholderFragment("two")} },
                {"{one}two", new ITemplateFragment[] { new PlaceholderFragment("one"), new TextFragment("two")} },
                {"one{two}", new ITemplateFragment[] { new TextFragment("one"), new PlaceholderFragment("two")} },

                {"{date:yyyy}/{date:MM} - {event}", new ITemplateFragment[] {
                    new PlaceholderFragment("date:yyyy"), new TextFragment("/"),
                    new PlaceholderFragment("date:MM"), new TextFragment(" - "),
                    new PlaceholderFragment("event")} },

                { "one\\{two", new [] {new TextFragment("one{two")} },
                { "one\\{two{hello}", new ITemplateFragment[] {new TextFragment("one{two"), new PlaceholderFragment("hello")} },
                { "one{two\\}wtf}", new ITemplateFragment[] {new TextFragment("one"), new PlaceholderFragment("two}wtf")} },
                { "one{two\\\\}wtf", new ITemplateFragment[] {new TextFragment("one"), new PlaceholderFragment("two\\"), new TextFragment("wtf")} },
            };

        [Theory]
        [MemberData(nameof(TestData))]
        public void TestParseTemplateString(string s, IEnumerable<ITemplateFragment> expected)
        {
            // act
            var result = TemplateStringParser.ParseFragments(s);

            // assert
            Assert.Collection(result, expected.Select(x => new Action<ITemplateFragment>(y =>
            {
                Assert.Equal(x.GetType(), y.GetType());
                Assert.Equal(x.Value, y.Value);
            })).ToArray());
        }
    }
}
