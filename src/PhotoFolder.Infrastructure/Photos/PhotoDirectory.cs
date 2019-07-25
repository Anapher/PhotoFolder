using PhotoFolder.Core.Dto.Services;
using PhotoFolder.Core.Interfaces.Gateways;
using PhotoFolder.Core.Interfaces.Gateways.Repositories;
using System;
using System.Collections.Generic;
using System.IO.Abstractions;
using System.Linq;
using System.Text;

namespace PhotoFolder.Infrastructure.Photos
{
    public class PhotoDirectory : IPhotoDirectory
    {
        private readonly IFileSystem _fileSystem;
        private readonly string _rootDirectory;

        public PhotoDirectory(IFileSystem fileSystem, string rootDirectory)
        {
            _fileSystem = fileSystem;
            _rootDirectory = rootDirectory;
        }

        // TODO Operating System settings
        public IEqualityComparer<string> PathComparer => StringComparer.OrdinalIgnoreCase;

        public IEnumerable<Core.Dto.Services.IFile> EnumerateFiles()
        {
            return _fileSystem
                .DirectoryInfo
                .FromDirectoryName(_rootDirectory)
                .EnumerateFiles("*", System.IO.SearchOption.AllDirectories)
                .Select(x => new Files.FileInfoWrapper(x, _rootDirectory));
        }

        public Core.Dto.Services.IFile? GetFile(string filename)
        {
            var fileInfo = _fileSystem.FileInfo.FromFileName(filename);
            if (!fileInfo.Exists) return null;

            return new Files.FileInfoWrapper(fileInfo, _rootDirectory);
        }

        public IIndexedFileRepository GetFileRepository()
        {
            throw new NotImplementedException();
        }

        public IFileOperationRepository GetOperationRepository()
        {
            throw new NotImplementedException();
        }
    }
}
