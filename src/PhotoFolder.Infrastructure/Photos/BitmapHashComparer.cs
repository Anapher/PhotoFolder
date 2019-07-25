using Microsoft.Extensions.Options;
using PhotoFolder.Core;
using PhotoFolder.Core.Interfaces.Services;
using PhotoFolder.Infrastructure.Utilities;

namespace PhotoFolder.Infrastructure.Photos
{
    public class BitmapHashComparer : IBitmapHashComparer
    {
        public BitmapHashComparer(IOptions<BitmapHashOptions> options)
        {
            RequiredBitmapHashEquality = options.Value.RequiredEqualityPercentage;
        }

        public float RequiredBitmapHashEquality { get; }

        public float Compare(Hash x, Hash y)
        {
            return BinaryUtils.ComputeByteArrayEquality(x.HashData, y.HashData);
        }
    }
}
