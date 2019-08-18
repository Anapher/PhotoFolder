using PhotoFolder.Core.Domain.Entities;
using System.Collections.Generic;

namespace PhotoFolder.Core.Dto.UseCaseResponses
{
    public class CheckFileIntegrityResponse
    {
        public CheckFileIntegrityResponse(IReadOnlyList<FileLocation> equalFiles, IReadOnlyList<SimilarFile> similarFiles,
            bool isWrongPlaced, IReadOnlyList<string>? recommendedDirectories, string? recommendedFilename)
        {
            EqualFiles = equalFiles;
            SimilarFiles = similarFiles;
            IsWrongPlaced = isWrongPlaced;
            RecommendedDirectories = recommendedDirectories;
            RecommendedFilename = recommendedFilename;
        }

        public IReadOnlyList<FileLocation> EqualFiles { get; }
        public IReadOnlyList<SimilarFile> SimilarFiles { get; }

        public bool IsWrongPlaced { get; }
        public IReadOnlyList<string>? RecommendedDirectories { get; }

        public string? RecommendedFilename { get; }
    }

    public class SimilarFile
    {
        public SimilarFile(IndexedFile indexedFile, float similarity)
        {
            IndexedFile = indexedFile;
            Similarity = similarity;
        }

        public IndexedFile IndexedFile { get; }
        public float Similarity { get; }
    }
}
