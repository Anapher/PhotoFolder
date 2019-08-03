namespace PhotoFolder.Infrastructure.Utilities
{
    public static class StringExtensions
    {
        public static string TrimStart(this string s, string trimString)
        {
            if (s.StartsWith(trimString))
                return s.Substring(trimString.Length);
            return s;
        }
    }
}
