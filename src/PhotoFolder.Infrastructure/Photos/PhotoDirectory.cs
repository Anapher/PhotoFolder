using PhotoFolder.Core.Domain.Entities;
using PhotoFolder.Core.Dto.Services;
using PhotoFolder.Core.Interfaces.Gateways;
using PhotoFolder.Core.Interfaces.Gateways.Repositories;
using PhotoFolder.Infrastructure.TemplatePath;
using System;
using System.Collections.Generic;
using System.IO.Abstractions;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace PhotoFolder.Infrastructure.Photos
{
    public class PhotoDirectory : IPhotoDirectory
    {
        private readonly IFileSystem _fileSystem;
        private readonly string _rootDirectory;
        private readonly TemplateString _photoFilenameTemplate;

        public PhotoDirectory(IFileSystem fileSystem, string rootDirectory, string photoFilenameTemplate)
        {
            _fileSystem = fileSystem;
            _rootDirectory = rootDirectory;

            _photoFilenameTemplate = TemplateString.Parse(photoFilenameTemplate);
        }

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

        public Regex GetFilePathMatcher(FileInformation fileInformation)
        {
            throw new NotImplementedException();
        }

        public string GetRecommendedPath(FileInformation fileInformation)
        {
            // fill palceholders
            // split \/
            // trim everything except alphanumerics
            // forge together with _fileSystem separator
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
