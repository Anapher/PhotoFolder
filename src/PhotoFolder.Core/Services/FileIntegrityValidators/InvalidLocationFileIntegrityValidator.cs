using PhotoFolder.Core.Dto.Services;
using PhotoFolder.Core.Dto.Services.FileIssue;
using PhotoFolder.Core.Extensions;
using PhotoFolder.Core.Interfaces.Gateways;
using PhotoFolder.Core.Interfaces.Services;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PhotoFolder.Core.Services.FileIntegrityValidators
{
    public class InvalidLocationFileIntegrityValidator : IFileIntegrityValidator
    {
        private readonly IPathUtils _pathUtils;

        public InvalidLocationFileIntegrityValidator(IPathUtils pathUtils)
        {
            _pathUtils = pathUtils;
        }

        public async ValueTask<IEnumerable<IFileIssue>> CheckForIssues(FileInformation file, IFileBaseContext fileBaseContext,
            IPhotoDirectoryDataContext dataContext)
        {
            var photoDirectory = fileBaseContext.PhotoDirectory;
            var pathPattern = photoDirectory.GetFilenameTemplate(file).ToRegexPattern();

            if (file.RelativeFilename == null || !Regex.IsMatch(file.RelativeFilename, pathPattern))
            {
                var recommendedPath = photoDirectory.GetRecommendedPath(file);
                var recommendedDirectory = _pathUtils.GetDirectoryName(recommendedPath);
                var recommendedName = _pathUtils.GetFileName(recommendedPath);

                var directoryTemplate = photoDirectory.GetFileDirectoryTemplate(file);

                var directorySuggestions = await dataContext.FileRepository.FindMatchingDirectories(directoryTemplate);
                if (!directorySuggestions.Contains(recommendedDirectory))
                    directorySuggestions.Add(recommendedDirectory);

                return new InvalidFileLocationIssue(file, directoryTemplate,
                    directorySuggestions.Select(x => new FilenameSuggestion(x, _pathUtils.Combine(x, recommendedName))).ToList(), recommendedName).Yield();
            }

            return Enumerable.Empty<IFileIssue>();
        }
    }
}
