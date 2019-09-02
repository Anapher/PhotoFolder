using System.Collections.Generic;
using PhotoFolder.Core.Domain.Entities;

namespace PhotoFolder.Infrastructure.Photos
{
    public interface ISimilarityDictionaryFactory
    {
        SimilarityDictionary Create(IReadOnlyList<IndexedFile> files);
    }
}