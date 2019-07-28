using PhotoFolder.Core.Domain.Entities;
using System.Collections.Generic;

namespace PhotoFolder.Core.Dto.UseCaseResponses
{
    public class CheckFileIntegrityResponse
    {
        public CheckFileIntegrityResponse(IReadOnlyList<FileLocation> equalFiles, IReadOnlyDictionary<IndexedFile, float> similarFiles,
            bool isWrongPlaced, IReadOnlyList<string>? recommendedDirectories, string? recommendedFilename)
        {
            EqualFiles = equalFiles;
            SimilarFiles = similarFiles;
            IsWrongPlaced = isWrongPlaced;
            RecommendedDirectories = recommendedDirectories;
            RecommendedFilename = recommendedFilename;
        }

        public IReadOnlyList<FileLocation> EqualFiles { get; }
        public IReadOnlyDictionary<IndexedFile, float> SimilarFiles { get; }

        public bool IsWrongPlaced { get; }
        public IReadOnlyList<string>? RecommendedDirectories { get; }

        public string? RecommendedFilename { get; }
    }
}
