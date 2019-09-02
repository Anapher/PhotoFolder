using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PhotoFolder.Core.Dto.Services;
using PhotoFolder.Core.Dto.Services.FileIssue;
using PhotoFolder.Core.Interfaces.Gateways;
using PhotoFolder.Core.Interfaces.Services;
using PhotoFolder.Core.Utilities;

namespace PhotoFolder.Core.Services.FileIntegrityValidators
{
    public class DuplicateFileIntegrityValidator : IFileIntegrityValidator
    {
        public ValueTask<IEnumerable<IFileIssue>> CheckForIssues(FileInformation file, IFileBaseContext fileBaseContext, IPhotoDirectoryDataContext dataContext)
        {
            return new ValueTask<IEnumerable<IFileIssue>>(CheckForIssues(file, fileBaseContext));
        }

        private IEnumerable<IFileIssue> CheckForIssues(FileInformation file, IFileBaseContext fileBaseContext)
        {
            if (fileBaseContext.TryGetIndexedFileByHash(file.Hash, out var indexedFile))
            {
                // don't include the actual file we're checking
                var equalFiles = indexedFile.Files
                    .Where(x => file.RelativeFilename == null || !fileBaseContext.PhotoDirectory.PathComparer.Equals(x.RelativeFilename, file.RelativeFilename))
                    .Select(x => indexedFile.ToFileInformation(x.RelativeFilename, fileBaseContext.PhotoDirectory)).ToList();

                if (equalFiles.Any())
                    yield return new DuplicateFilesIssue(file, equalFiles);
            }
        }
    }
}
