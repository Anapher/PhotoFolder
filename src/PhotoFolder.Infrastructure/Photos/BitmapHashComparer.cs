using Microsoft.Extensions.Options;
using PhotoFolder.Core.Interfaces.Services;
using PhotoFolder.Infrastructure.Utilities;
using System.Collections.Generic;
using System.Linq;
using PhotoFolder.Core.Domain;

namespace PhotoFolder.Infrastructure.Photos
{
    public class BitmapHashComparer : IBitmapHashComparer
    {
        public BitmapHashComparer(IOptions<InfrastructureOptions> options)
        {
            RequiredBitmapHashEquality = options.Value.RequiredSimilarityForEquality;
        }

        public float RequiredBitmapHashEquality { get; }

        public float Compare(Hash x, Hash y)
        {
            var hashVariants = new List<byte[]> { x.HashData }; // 0°
            hashVariants.Add(BitmapHash.RotateBitmapHash(hashVariants.Last())); // 90°
            hashVariants.Add(BitmapHash.RotateBitmapHash(hashVariants.Last())); // 180°
            hashVariants.Add(BitmapHash.RotateBitmapHash(hashVariants.Last())); // 270°

            return hashVariants.Select(x => BinaryUtils.ComputeByteArrayEquality(x, y.HashData)).Max();
        }
    }
}
