using PhotoFolder.Core.Domain.Entities;
using PhotoFolder.Core.Dto.Services;
using PhotoFolder.Core.Dto.Services.FileIssue;
using PhotoFolder.Core.Extensions;
using PhotoFolder.Core.Interfaces.Gateways;
using PhotoFolder.Core.Interfaces.Services;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace PhotoFolder.Core.Services.FileIntegrityValidators
{
    public class InvalidLocationFileIntegrityValidator : IFileIntegrityValidator
    {
        public IEnumerable<IFileIssue> CheckForIssues(FileInformation file, IPhotoDirectory photoDirectory, IReadOnlyList<IndexedFile> indexedFiles)
        {
            var pathPattern = photoDirectory.GetFilenameRegexPattern(file);

            if (!file.IsRelativeFilename || !Regex.IsMatch(file.Filename, pathPattern))
            {
                var recommendedPath = photoDirectory.GetRecommendedPath(file);
                var recommendedDirectory = Path.GetDirectoryName(recommendedPath);
                var recommendedName = Path.GetFileName(recommendedPath);

                var directoryPattern = photoDirectory.GetFileDirectoryRegexPattern(file);
                var directoryRegex = new Regex(directoryPattern);

                var directorySuggestions = indexedFiles.SelectMany(x => x.Files).Select(x => Path.GetDirectoryName(x.Filename))
                    .Where(x => directoryRegex.IsMatch(x)).Concat(recommendedDirectory.Yield()).Distinct().ToList();

                yield return new InvalidFileLocationIssue(file, directoryPattern,
                    directorySuggestions.Select(x => new FilenameSuggestion(x, Path.Combine(x, recommendedName))).ToList(), recommendedName);
            }
        }
    }
}
