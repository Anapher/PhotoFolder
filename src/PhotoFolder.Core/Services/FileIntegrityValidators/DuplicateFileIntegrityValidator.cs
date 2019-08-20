using PhotoFolder.Core.Domain.Entities;
using PhotoFolder.Core.Dto.Services;
using PhotoFolder.Core.Dto.Services.FileIssue;
using PhotoFolder.Core.Interfaces.Gateways;
using PhotoFolder.Core.Interfaces.Services;
using System.Collections.Generic;
using System.Linq;
using PhotoFolder.Core.Utilities;

namespace PhotoFolder.Core.Services.FileIntegrityValidators
{
    public class DuplicateFileIntegrityValidator : IFileIntegrityValidator
    {
        public IEnumerable<IFileIssue> CheckForIssues(FileInformation file, IPhotoDirectory photoDirectory, IReadOnlyList<IndexedFile> indexedFiles)
        {
            // files are indexed by their hash, so there can be only one indexed file with the same hash
            var indexedFile = indexedFiles.FirstOrDefault(x => x.Hash == file.Hash);
            if (indexedFile != null)
            {
                // dont include the actual file we're checking
                var equalFiles = indexedFile.Files.Where(x => !photoDirectory.PathComparer.Equals(x.Filename, file.Filename))
                    .Select(x => indexedFile.ToFileInformation(x.Filename)).ToList();

                yield return new DuplicateFilesIssue(file, equalFiles);
            }
        }
    }
}
