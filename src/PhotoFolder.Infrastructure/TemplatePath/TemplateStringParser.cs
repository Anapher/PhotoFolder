using System.Collections.Generic;
using System.Text;

namespace PhotoFolder.Infrastructure.TemplatePath
{
    public static class TemplateStringParser
    {
        private const char PlaceholderStartChar = '{';
        private const char PlaceholderEndChar = '}';
        private const char EscapeChar = '\\';

        public static IEnumerable<ITemplateFragment> ParseFragments(string s)
        {
            var index = 0;
            var lastTokenIndex = 0;

            do
            {
                if (s[index] == PlaceholderStartChar && (index == 0 || (s[index - 1] != EscapeChar)))
                {
                    if (lastTokenIndex != index)
                    {
                        yield return new TextFragment(ClearEscapeChars(s.Substring(lastTokenIndex,
                            index - lastTokenIndex)));
                    }

                    var content = ReadToken(s, PlaceholderEndChar, ref index);
                    yield return new PlaceholderFragment(ClearEscapeChars(content));
                }
                else
                {
                    continue;
                }

                lastTokenIndex = index + 1;
            } while (++index < s.Length);

            if (lastTokenIndex != s.Length)
                yield return new TextFragment(ClearEscapeChars(s.Substring(lastTokenIndex, index - lastTokenIndex)));
        }

        private static string ClearEscapeChars(string s)
        {
            var sb = new StringBuilder();

            var isNextEscaped = false;
            foreach (var c in s)
            {
                if (isNextEscaped)
                {
                    sb.Append(c);
                    isNextEscaped = false;
                    continue;
                }

                if (c == EscapeChar)
                {
                    isNextEscaped = true;
                    continue;
                }

                sb.Append(c);
            }

            return sb.ToString();
        }

        private static string ReadToken(string value, char endChar, ref int i)
        {
            var buf = value;
            var valueLength = value.Length;
            var tokenStartPos = i;
            var isNextEscaped = false;

            while (++i < valueLength)
            {
                var valueChar = buf[i];

                if (isNextEscaped)
                {
                    isNextEscaped = false;
                    continue;
                }

                if (valueChar == EscapeChar)
                {
                    isNextEscaped = true;
                    continue;
                }

                if (valueChar == endChar)
                {
                    break;
                }
            }

            return value.Substring(tokenStartPos + 1, i - tokenStartPos - 1);
        }
    }
}
