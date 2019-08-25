using PhotoFolder.Core.Domain.Entities;
using PhotoFolder.Core.Dto.Services;
using PhotoFolder.Core.Dto.Services.FileIssue;
using PhotoFolder.Core.Interfaces.Gateways;
using PhotoFolder.Core.Interfaces.Services;
using System.Collections.Generic;
using System.Linq;
using PhotoFolder.Core.Domain;
using PhotoFolder.Core.Utilities;

namespace PhotoFolder.Core.Services.FileIntegrityValidators
{
    public class DuplicateFileIntegrityValidator : IFileIntegrityValidator
    {
        public IEnumerable<IFileIssue> CheckForIssues(FileInformation file, IPhotoDirectory photoDirectory, IReadOnlyList<IndexedFile> indexedFiles)
        {
            // files are indexed by their hash, so there can be only one indexed file with the same hash
            var indexedFile = indexedFiles.FirstOrDefault(x => Hash.Parse(x.Hash).Equals(file.Hash));
            if (indexedFile != null)
            {
                // don't include the actual file we're checking
                var equalFiles = indexedFile.Files.Where(x => file.RelativeFilename == null || !photoDirectory.PathComparer.Equals(x.RelativeFilename, file.RelativeFilename))
                    .Select(x => indexedFile.ToFileInformation(x.RelativeFilename, photoDirectory)).ToList();

                if (equalFiles.Any())
                    yield return new DuplicateFilesIssue(file, equalFiles);
            }
        }
    }
}
