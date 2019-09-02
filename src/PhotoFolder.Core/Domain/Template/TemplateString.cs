using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text.RegularExpressions;
using PhotoFolder.Core.Services;

namespace PhotoFolder.Core.Domain.Template
{
    public class TemplateString
    {
        private readonly IReadOnlyList<ITemplateFragment> _fragments;

        public TemplateString(IEnumerable<ITemplateFragment> fragments)
        {
            _fragments = fragments.ToList();
        }

        public IEnumerable<ITemplateFragment> Fragments => _fragments;

        public static TemplateString Parse(string s)
        {
            var fragments = TemplateStringParser.ParseFragments(s);
            return new TemplateString(fragments);
        }

        public string ToRegexPattern()
        {
            return "^" + ToPattern(Regex.Escape, ".*?") + "$";
        }

        public string ToLikePattern(char escapeChar = '\\')
        {
            var specialChars = new [] {"%", "_", "["};

            return ToPattern(x => specialChars.Aggregate(x, (current, specialChar) => current.Replace(specialChar, escapeChar + specialChar)), "%");
        }

        private string ToPattern(Func<string, string> escapeText, string placeholderPattern)
        {
            return Fragments.Aggregate(string.Empty, (s, x) =>
            {
                if (x is TextFragment)
                {
                    if (_fragments.SkipWhile(y => y != x).Skip(1).FirstOrDefault() is PlaceholderFragment)
                        return s + escapeText(x.Value.TrimEnd(' ', '-'));
                    return s + escapeText(x.Value);
                }

                return s + placeholderPattern;
            });
        }

        public string ToString(IReadOnlyDictionary<string, string> placeholderValues)
        {
            return Fragments.Aggregate("", (s, x) => {
                if (x is TextFragment)
                    return s + x.Value;
                if (placeholderValues.TryGetValue(x.Value, out var placeholder))
                    return s + placeholder;
                return s + "{" + x.Value + "}";
            });
        }

        public override string ToString()
        {
            return ToString(ImmutableDictionary<string, string>.Empty);
        }
    }
}
