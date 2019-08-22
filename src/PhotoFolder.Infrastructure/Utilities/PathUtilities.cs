using System;
using System.Linq;

namespace PhotoFolder.Infrastructure.Utilities
{
    public static class PathUtilities
    {
        public static string ToForwardSlashes(this string path) => path.Replace('\\', '/');

        /// <summary>
        ///     Apply the modifiers on every part of the path and return the result
        /// </summary>
        /// <param name="path">The path that should be patched</param>
        /// <param name="separators">The separators the path should be splitted by. The first entry will be used to build up the final path</param>
        /// <param name="modifiers">The modifier functions</param>
        /// <returns>Return the patched path</returns>
        public static string PatchPathParts(string path, char[] separators, params Func<string, string>[] modifiers)
        {
            var parts = path.Split(separators);
            var fixedParts = parts.Select(x => modifiers.Aggregate(x, (s, fn) => fn(s)));

            return string.Join(separators[0], fixedParts);
        }

        public static Func<string, string> TrimChars(params char[] trimmedChars)
        {
            return s => s.Trim(trimmedChars);
        }

        public static Func<string, string> RemoveInvalidChars(params char[] invalidChars)
        {
            return s => new string(s.ToCharArray().Where(x => !invalidChars.Contains(x)).ToArray());
        }
    }
}
