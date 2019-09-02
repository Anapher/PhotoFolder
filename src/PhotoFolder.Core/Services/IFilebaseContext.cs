using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using PhotoFolder.Core.Domain;
using PhotoFolder.Core.Domain.Entities;
using PhotoFolder.Core.Interfaces.Gateways;

namespace PhotoFolder.Core.Services
{
    public interface IFileBaseContext
    {
        IPhotoDirectory PhotoDirectory { get; }
        IReadOnlyList<IndexedFile> IndexedFiles { get; }

        bool TryGetIndexedFileByHash(Hash hash, [NotNullWhen(true)] out IndexedFile? indexedFile);
        IEnumerable<(IndexedFile, float)> FindSimilarFiles(Hash bitmapHash);
    }
}
