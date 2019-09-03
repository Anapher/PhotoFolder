using System;
using PhotoFolder.Infrastructure.Utilities;
using PhotoFolder.Core.Domain;
using PhotoFolder.Core.Dto.Services;
using PhotoFolder.Core.Interfaces.Services;

namespace PhotoFolder.Infrastructure.Photos
{
    public class BitmapHashComparer : IBitmapHashComparer
    {
        //public float Compare(Hash x, Hash y)
        //{
        //    var hashVariants = new List<byte[]> { x.HashData }; // 0°
        //    hashVariants.Add(BitmapHash.RotateBitmapHash(hashVariants.Last())); // 90°
        //    hashVariants.Add(BitmapHash.RotateBitmapHash(hashVariants.Last())); // 180°
        //    hashVariants.Add(BitmapHash.RotateBitmapHash(hashVariants.Last())); // 270°

        //    return hashVariants.Select(x => BinaryUtils.ComputeByteArrayEquality(x, y.HashData)).Max();
        //}

        public HashContext CreateContext(Hash hash)
        {
            var variants = new ReadOnlyMemory<byte>[4];
            variants[0] = hash.HashData.AsMemory(0, 32);
            variants[1] = BitmapHash.RotateBitmapHash(variants[0].Span); // 90°
            variants[2] = BitmapHash.RotateBitmapHash(variants[1].Span); // 180°
            variants[3] = BitmapHash.RotateBitmapHash(variants[2].Span); // 270°

            var colorData = hash.HashData.AsMemory(32);
            return new HashContext(colorData, variants);
        }

        public float Compare(HashContext context, Hash bitmapHash)
        {
            var colorData = bitmapHash.HashData.AsSpan(32);
            var colorFactor = ComputeColorFactor(colorData, context.ColorData.Span);

            var bitmapHashData = bitmapHash.HashData.AsSpan(0, 32);

            float bestSimilarity = 0;
            foreach (var variant in context.Variants)
            {
                var similarity = BinaryUtils.ComputeByteArrayEquality(variant.Span, bitmapHashData);
                bestSimilarity = Math.Max(bestSimilarity, similarity);
            }

            return bestSimilarity * colorFactor;
        }

        private static float ComputeColorFactor(ReadOnlySpan<byte> color1, ReadOnlySpan<byte> color2)
        {
            var diff = Math.Abs(color1[0] - color2[0]) + Math.Abs(color1[1] - color2[1]) + Math.Abs(color1[2] - color2[2]);
            var maxDiff = byte.MaxValue * 3;

            return (maxDiff - diff) / (float) maxDiff;
        }
    }
}
