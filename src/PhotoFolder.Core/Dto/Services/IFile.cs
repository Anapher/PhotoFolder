using System;
using System.IO;

namespace PhotoFolder.Core.Dto.Services
{
    public interface IFile : IFileInfo
    {
        Stream OpenRead();
    }
}
