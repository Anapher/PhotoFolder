using PhotoFolder.Core.Dto.Services;
using PhotoFolder.Core.Interfaces.Services;
using System.Collections.Generic;

namespace PhotoFolder.Core.Services
{
    public class FileContentInfoComparer : IEqualityComparer<IFileContentInfo>
    {
        private readonly IBitmapHashComparer _bitmapHashComparer;

        public FileContentInfoComparer(IBitmapHashComparer bitmapHashComparer)
        {
            _bitmapHashComparer = bitmapHashComparer;
        }

        public bool Equals(IFileContentInfo file1, IFileContentInfo file2)
        {
            // hash equality
            if (file1.Hash.Equals(file2.Hash))
                return true;

            if (file1.PhotoProperties == null || file2.PhotoProperties == null)
                return false; // we can only compare images

            if (_bitmapHashComparer.Compare(file1.PhotoProperties.BitmapHash,
                file2.PhotoProperties.BitmapHash) >= _bitmapHashComparer.RequiredBitmapHashEquality)
                return true;

            return false;
        }

        public int GetHashCode(IFileContentInfo obj)
        {
            return obj.Hash.GetHashCode();
        }
    }
}
