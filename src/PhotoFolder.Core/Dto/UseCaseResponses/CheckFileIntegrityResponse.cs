using PhotoFolder.Core.Domain.Entities;
using System.Collections.Generic;

namespace PhotoFolder.Core.Dto.UseCaseResponses
{
    public class CheckFileIntegrityResponse
    {
        public IEnumerable<FileLocation> EqualFiles { get; }
        public IDictionary<IndexedFile, float> SimilarFiles { get; }
        public string? RecommendedFilename { get; }
    }
}
