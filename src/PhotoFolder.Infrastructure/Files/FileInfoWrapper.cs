using System;
using System.IO;
using PhotoFolder.Core.Dto.Services;
using PhotoFolder.Infrastructure.Utilities;
using IFileInfo = System.IO.Abstractions.IFileInfo;

namespace PhotoFolder.Infrastructure.Files
{
    public class FileInfoWrapper : IFile
    {
        private readonly IFileInfo _fileInfo;

        public FileInfoWrapper(IFileInfo fileInfo, string? relativeFilename)
        {
            _fileInfo = fileInfo;

            Filename = fileInfo.FullName.ToForwardSlashes();
            RelativeFilename = relativeFilename;
        }

        public string Filename { get; }
        public string? RelativeFilename { get; }
        public long Length => _fileInfo.Length;
        public DateTimeOffset CreatedOn => _fileInfo.CreationTimeUtc;
        public DateTimeOffset ModifiedOn => _fileInfo.LastWriteTimeUtc;

        public Stream OpenRead() => _fileInfo.OpenRead();
    }
}
