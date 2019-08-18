using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using PhotoFolder.Core.Dto.Services;

namespace PhotoFolder.Infrastructure.Photos
{
    public static class FilePlaceholderFiller
    {
        private static readonly IReadOnlyDictionary<string, Func<FileInformation, string, string?>> _placeholderMap
            = new Dictionary<string, Func<FileInformation, string, string?>> {
                { "hash", (x, param) => FormatString(x.Hash, param) },
                { "filename", (x, param) => FormatString(Path.GetFileName(x.Filename), param)},
                { "width", (x, _) => x.PhotoProperties?.Width.ToString() ?? string.Empty },
                { "height", (x, _) => x.PhotoProperties?.Height.ToString() ?? string.Empty },
                { "date", (x, param) => FormatDateTimeOffset(x.FileCreatedOn, param) }
            };

        public static IDictionary<string, string?> GetPlaceholders(IEnumerable<string> placeholders,
            FileInformation fileInformation)
        {
            return placeholders.ToDictionary(x => x, x => TryGetPlaceholder(x, fileInformation));
        }

        private static string? TryGetPlaceholder(string key, FileInformation fileInformation)
        {
            var parameters = key.Split(':', 2);

            if (_placeholderMap.TryGetValue(parameters[0], out var placeholderFn))
                return placeholderFn(fileInformation, parameters.Skip(1).FirstOrDefault());

            return null;
        }

        private static string FormatString(string s, string? param)
        {
            switch (param)
            {
                case "tolower":
                    return s.ToLowerInvariant();
                case "toupper":
                    return s.ToUpperInvariant();
                default:
                    return s;
            }
        }

        private static string FormatDateTimeOffset(DateTimeOffset date, string? param)
        {
            if (param == null)
                return date.ToString();

            return date.ToString(param);
        }
    }
}