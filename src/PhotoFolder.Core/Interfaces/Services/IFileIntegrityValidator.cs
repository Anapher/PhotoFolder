using PhotoFolder.Core.Domain.Entities;
using PhotoFolder.Core.Dto.Services;
using PhotoFolder.Core.Interfaces.Gateways;
using System.Collections.Generic;

namespace PhotoFolder.Core.Interfaces.Services
{
    public interface IFileIntegrityValidator
    {
        IEnumerable<IFileIssue> CheckForIssues(FileInformation file, IPhotoDirectory photoDirectory, IReadOnlyList<IndexedFile> indexedFiles);
    }
}
