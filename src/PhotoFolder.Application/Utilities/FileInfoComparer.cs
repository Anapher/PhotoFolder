using PhotoFolder.Core.Dto.Services;
using System;
using System.Collections.Generic;

namespace PhotoFolder.Application.Utilities
{
    public class FileInfoComparer : IEqualityComparer<IFileInfo>
    {
        public bool Equals(IFileInfo x, IFileInfo y)
        {
            return x.Filename == y.Filename && x.Length == y.Length && x.CreatedOn == y.CreatedOn && x.ModifiedOn == y.ModifiedOn;
        }

        public int GetHashCode(IFileInfo obj)
        {
            return HashCode.Combine(obj.Filename.Length, obj.Length.GetHashCode(), obj.CreatedOn.GetHashCode(), obj.ModifiedOn.GetHashCode());
        }
    }
}
