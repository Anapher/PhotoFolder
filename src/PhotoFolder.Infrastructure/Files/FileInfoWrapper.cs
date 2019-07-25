using PhotoFolder.Core.Dto.Services;
using System;
using System.IO;

namespace PhotoFolder.Infrastructure.Files
{
    public class FileInfoWrapper : IFile
    {
        private readonly System.IO.Abstractions.IFileInfo _fileInfo;
        private readonly string _rootDirectory;

        public FileInfoWrapper(System.IO.Abstractions.IFileInfo fileInfo, string rootDirectory)
        {
            _fileInfo = fileInfo;
            _rootDirectory = rootDirectory;
        }

        public string Filename => _fileInfo.FullName.Substring(_rootDirectory.Length + 1);
        public long Length => _fileInfo.Length;
        public DateTimeOffset CreatedOn => _fileInfo.CreationTimeUtc;
        public DateTimeOffset ModifiedOn => _fileInfo.LastWriteTimeUtc;

        public Stream OpenRead() => _fileInfo.OpenRead();
    }
}
