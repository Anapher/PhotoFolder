using PhotoFolder.Core.Domain.Entities;
using System.Collections.Generic;

namespace PhotoFolder.Core.Dto.UseCaseResponses
{
    public class CheckFileIntegrityResponse
    {
        public CheckFileIntegrityResponse(IEnumerable<FileLocation> equalFiles, IDictionary<IndexedFile, float> similarFiles, string recommendedFilename)
        {
            EqualFiles = equalFiles;
            SimilarFiles = similarFiles;
            RecommendedFilename = recommendedFilename;
        }

        public IEnumerable<FileLocation> EqualFiles { get; }
        public IDictionary<IndexedFile, float> SimilarFiles { get; }
        public string? RecommendedFilename { get; }
    }
}
