using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace PhotoFolder.Infrastructure.TemplatePath
{
    public class TemplateString
    {
        public TemplateString(IEnumerable<ITemplateFragment> fragments)
        {
            Fragments = fragments.ToList();
        }

        public IEnumerable<ITemplateFragment> Fragments { get; }

        public static TemplateString Parse(string s)
        {
            var fragments = TemplateStringParser.ParseFragments(s);
            return new TemplateString(fragments);
        }

        public string ToRegexPattern()
        {
            return Fragments.Aggregate("", (s, x) =>
            {
                if (x is TextFragment)
                    return s + Regex.Escape(x.Value);

                return s + ".+?";
            });
        }

        public string ToString(Dictionary<string, string> placeholderValues)
        {
            return Fragments.Aggregate("", (s, x) => {
                if (x is TextFragment)
                    return s + x;
                if (placeholderValues.TryGetValue(x.Value, out var placeholder))
                    return s + placeholder;
                return "{" + x.Value + "}";
            });
        }

        public override string ToString()
        {
            return Fragments.Aggregate("", (s, x) => s + (x is PlaceholderFragment ? "{" + x.Value + "}" : x.Value));
        }
    }
}