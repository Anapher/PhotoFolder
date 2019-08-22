﻿using PhotoFolder.Core.Dto.Services;
using PhotoFolder.Infrastructure.Utilities;
using System;
using System.IO;

namespace PhotoFolder.Infrastructure.Files
{
    public class FileInfoWrapper : IFile
    {
        private readonly System.IO.Abstractions.IFileInfo _fileInfo;

        public FileInfoWrapper(System.IO.Abstractions.IFileInfo fileInfo, string? rootDirectory = null)
        {
            _fileInfo = fileInfo;

            if (rootDirectory == null)
                Filename = fileInfo.FullName.ToForwardSlashes();
            else Filename = fileInfo.FullName.TrimStart(rootDirectory + fileInfo.FileSystem.Path.DirectorySeparatorChar).ToForwardSlashes();
        }

        public string Filename { get; }
        public long Length => _fileInfo.Length;
        public DateTimeOffset CreatedOn => _fileInfo.CreationTimeUtc;
        public DateTimeOffset ModifiedOn => _fileInfo.LastWriteTimeUtc;

        public bool IsRelativeFilename => _fileInfo.FullName.Length != Filename.Length;

        public Stream OpenRead() => _fileInfo.OpenRead();
    }
}
