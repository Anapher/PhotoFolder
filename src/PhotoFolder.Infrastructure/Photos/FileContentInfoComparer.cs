using System.Collections.Generic;
using Microsoft.Extensions.Options;
using PhotoFolder.Core.Dto.Services;
using PhotoFolder.Core.Interfaces.Services;

namespace PhotoFolder.Infrastructure.Photos
{
    public class FileContentInfoComparer : IEqualityComparer<IFileContentInfo>
    {
        private readonly IBitmapHashComparer _bitmapHashComparer;
        private readonly InfrastructureOptions _options;

        public FileContentInfoComparer(IBitmapHashComparer bitmapHashComparer, IOptions<InfrastructureOptions> options)
        {
            _bitmapHashComparer = bitmapHashComparer;
            _options = options.Value;
        }

        public bool Equals(IFileContentInfo file1, IFileContentInfo file2)
        {
            // hash equality
            if (file1.Hash.Equals(file2.Hash))
                return true;

            if (file1.PhotoProperties == null || file2.PhotoProperties == null)
                return false; // we can only compare images

            var context = _bitmapHashComparer.CreateContext(file1.PhotoProperties.BitmapHash);

            if (_bitmapHashComparer.Compare(context, file2.PhotoProperties.BitmapHash) >= _options.RequiredSimilarityForEquality)
                return true;

            return false;
        }

        public int GetHashCode(IFileContentInfo obj)
        {
            return obj.Hash.GetHashCode();
        }
    }
}
