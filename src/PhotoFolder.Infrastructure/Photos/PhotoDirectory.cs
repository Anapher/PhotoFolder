using PhotoFolder.Core.Domain.Entities;
using PhotoFolder.Core.Dto.Services;
using PhotoFolder.Core.Interfaces.Gateways;
using PhotoFolder.Core.Interfaces.Gateways.Repositories;
using PhotoFolder.Infrastructure.TemplatePath;
using PhotoFolder.Infrastructure.Utilities;
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

        public string GetFilePathRegexPattern(FileInformation fileInformation)
        {
            var values = FilePlaceholderFiller.GetPlaceholders(_photoFilenameTemplate.Fragments.OfType<PlaceholderFragment>().Select(x => x.Value), fileInformation);
            var path = _photoFilenameTemplate.ToString(values.Where(x => x.Value != null).ToDictionary(x => x.Key, x => x.Value!));

            return TemplateString.Parse(path).ToRegexPattern();
        }

        public string GetRecommendedPath(FileInformation fileInformation)
        {
            var values = FilePlaceholderFiller.GetPlaceholders(_photoFilenameTemplate.Fragments.OfType<PlaceholderFragment>().Select(x => x.Value), fileInformation);
            var path = _photoFilenameTemplate.ToString(values.ToDictionary(x => x.Key, x => x.Value ?? string.Empty));

            var dirSeparators = new[] { _fileSystem.Path.DirectorySeparatorChar, _fileSystem.Path.AltDirectorySeparatorChar };
            return PathUtilities.PatchPathParts(path, dirSeparators, PathUtilities.TrimChars(' ', '-'),
                PathUtilities.RemoveInvalidChars(_fileSystem.Path.GetInvalidFileNameChars()));
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
