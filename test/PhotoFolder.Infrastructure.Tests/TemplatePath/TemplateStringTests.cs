﻿using PhotoFolder.Infrastructure.TemplatePath;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace PhotoFolder.Infrastructure.Tests.TemplatePath
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
    }
}