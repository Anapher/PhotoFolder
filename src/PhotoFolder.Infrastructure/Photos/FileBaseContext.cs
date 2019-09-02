using System.Collections.Generic;
using PhotoFolder.Core.Domain;
using PhotoFolder.Core.Domain.Entities;
using PhotoFolder.Core.Interfaces.Gateways;
using PhotoFolder.Core.Services;

namespace PhotoFolder.Infrastructure.Photos
{
    public class FileBaseContext : IFileBaseContext
    {
        private readonly IReadOnlyDictionary<string, IndexedFile> _indexedFileMap;
        private readonly SimilarityDictionary _similarityDictionary;

        public FileBaseContext(IPhotoDirectory photoDirectory, IReadOnlyList<IndexedFile> indexedFiles, IReadOnlyDictionary<string, IndexedFile> indexedFileMap,
            SimilarityDictionary similarityDictionary)
        {
            _indexedFileMap = indexedFileMap;
            _similarityDictionary = similarityDictionary;
            PhotoDirectory = photoDirectory;
            IndexedFiles = indexedFiles;
        }

        public IPhotoDirectory PhotoDirectory { get; }
        public IReadOnlyList<IndexedFile> IndexedFiles { get; }

        public bool TryGetIndexedFileByHash(Hash hash, out IndexedFile? indexedFile)
        {
            return _indexedFileMap.TryGetValue(hash.ToString(), out indexedFile);
        }

        public IEnumerable<(IndexedFile, float)> FindSimilarFiles(Hash bitmapHash) => _similarityDictionary.FindSimilarFiles(bitmapHash);
    }
}
