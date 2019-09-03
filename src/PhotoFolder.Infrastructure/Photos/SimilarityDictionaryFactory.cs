using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Options;
using PhotoFolder.Core.Domain.Entities;
using PhotoFolder.Core.Interfaces.Services;
using PhotoFolder.Infrastructure.Utilities;

namespace PhotoFolder.Infrastructure.Photos
{
    public class SimilarityDictionaryFactory : ISimilarityDictionaryFactory
    {
        private readonly IBitmapHashComparer _bitmapHashComparer;
        private readonly InfrastructureOptions _options;

        public SimilarityDictionaryFactory(IOptions<InfrastructureOptions> options, IBitmapHashComparer bitmapHashComparer)
        {
            _bitmapHashComparer = bitmapHashComparer;
            _options = options.Value;
        }

        public SimilarityDictionary Create(IReadOnlyList<IndexedFile> files)
        {
            var dictionary = new Dictionary<byte, IList<IndexedFile>>();

            foreach (var indexedFile in files.Where(x => x.PhotoProperties != null))
            {
                var setBits = (byte) indexedFile.PhotoProperties!.BitmapHash.HashData.Sum(x => BinaryUtils.SumSetBits(x));
                if (dictionary.TryGetValue(setBits, out var indexedFiles))
                    indexedFiles.Add(indexedFile);
                else dictionary.Add(setBits, new List<IndexedFile> {indexedFile});
            }

            return new SimilarityDictionary(_options.RequiredSimilarityForIssue, dictionary, _bitmapHashComparer);
        }
    }
}
