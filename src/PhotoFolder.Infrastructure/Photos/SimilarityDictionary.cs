using System;
using System.Collections.Generic;
using System.Linq;
using PhotoFolder.Core.Domain;
using PhotoFolder.Core.Domain.Entities;
using PhotoFolder.Infrastructure.Utilities;

namespace PhotoFolder.Infrastructure.Photos
{
    public class SimilarityDictionary
    {
        private readonly float _requiredSimilarity;
        private readonly IReadOnlyDictionary<byte, IList<IndexedFile>> _indexedFiles;

        public SimilarityDictionary(float requiredSimilarity, IReadOnlyDictionary<byte, IList<IndexedFile>> indexedFiles)
        {
            _requiredSimilarity = requiredSimilarity;
            _indexedFiles = indexedFiles;
        }

        public IEnumerable<(IndexedFile, float)> FindSimilarFiles(Hash bitmapHash)
        {
            var hashData = bitmapHash.HashData;
            var setBits = hashData.Sum(x => BinaryUtils.SumSetBits(x));

            var maxPercentageDiff = 1 - _requiredSimilarity;
            var maxDiff = (int) (setBits * maxPercentageDiff); // floor

            var minBitsSet = Math.Max(setBits - maxDiff, 0);
            var maxBitsSet = Math.Min(setBits + maxDiff, 255);

            var hashVariants = new List<byte[]> { bitmapHash.HashData }; // 0°
            hashVariants.Add(BitmapHash.RotateBitmapHash(hashVariants.Last())); // 90°
            hashVariants.Add(BitmapHash.RotateBitmapHash(hashVariants.Last())); // 180°
            hashVariants.Add(BitmapHash.RotateBitmapHash(hashVariants.Last())); // 270°

            for (var i = minBitsSet; i <= maxBitsSet; i++)
            {
                if (_indexedFiles.TryGetValue((byte) i, out var files))
                {
                    foreach (var indexedFile in files)
                    {
                        var similarity = hashVariants.Select(x => BinaryUtils.ComputeByteArrayEquality(x, indexedFile.PhotoProperties!.BitmapHash.HashData)).Max();
                        if (similarity >= _requiredSimilarity)
                            yield return (indexedFile, similarity);
                    }
                }
            }
        }
    }
}
