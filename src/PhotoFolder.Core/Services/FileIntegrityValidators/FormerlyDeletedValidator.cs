using PhotoFolder.Core.Domain.Entities;
using PhotoFolder.Core.Dto.Services;
using PhotoFolder.Core.Dto.Services.FileIssue;
using PhotoFolder.Core.Interfaces.Gateways;
using PhotoFolder.Core.Interfaces.Services;
using System.Collections.Generic;

namespace PhotoFolder.Core.Services.FileIntegrityValidators
{
    public class FormerlyDeletedValidator : IFileIntegrityValidator
    {
        public IEnumerable<IFileIssue> CheckForIssues(FileInformation file, IPhotoDirectory photoDirectory, IReadOnlyList<IndexedFile> indexedFiles)
        {
            if (photoDirectory.DeletedFiles.Files.TryGetValue(file.Hash.ToString(), out var deletedFile))
            {
                yield return new FormerlyDeletedIssue(file, deletedFile);
            }
        }
    }
}
