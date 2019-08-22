using PhotoFolder.Core.Domain.Entities;
using PhotoFolder.Core.Dto.Services;
using PhotoFolder.Core.Dto.Services.FileIssue;
using PhotoFolder.Core.Extensions;
using PhotoFolder.Core.Interfaces.Gateways;
using PhotoFolder.Core.Interfaces.Services;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace PhotoFolder.Core.Services.FileIntegrityValidators
{
    public class InvalidLocationFileIntegrityValidator : IFileIntegrityValidator
    {
        private readonly IPathUtils _pathUtils;

        public InvalidLocationFileIntegrityValidator(IPathUtils pathUtils)
        {
            _pathUtils = pathUtils;
        }

        public IEnumerable<IFileIssue> CheckForIssues(FileInformation file, IPhotoDirectory photoDirectory, IReadOnlyList<IndexedFile> indexedFiles)
        {
            var pathPattern = photoDirectory.GetFilenameTemplate(file).ToRegexPattern();

            if (!file.IsRelativeFilename || !Regex.IsMatch(file.Filename, pathPattern))
            {
                var recommendedPath = photoDirectory.GetRecommendedPath(file);
                var recommendedDirectory = _pathUtils.GetDirectoryName(recommendedPath);
                var recommendedName = _pathUtils.GetFileName(recommendedPath);

                var directoryTemplate = photoDirectory.GetFileDirectoryTemplate(file);
                var directoryRegex = new Regex(directoryTemplate.ToRegexPattern());

                var directorySuggestions = indexedFiles.SelectMany(x => x.Files).Select(x => _pathUtils.GetDirectoryName(x.Filename))
                    .Where(x => directoryRegex.IsMatch(x)).Concat(recommendedDirectory.Yield()).Distinct().ToList();

                yield return new InvalidFileLocationIssue(file, directoryTemplate,
                    directorySuggestions.Select(x => new FilenameSuggestion(x, _pathUtils.Combine(x, recommendedName))).ToList(), recommendedName);
            }
        }
    }
}
