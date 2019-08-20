using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using PhotoFolder.Core.Dto.Services;
using PhotoFolder.Core.Interfaces.Gateways;
using PhotoFolder.Infrastructure.Consts;
using PhotoFolder.Infrastructure.Data;
using PhotoFolder.Infrastructure.TemplatePath;
using PhotoFolder.Infrastructure.Utilities;
using FileInfoWrapper = PhotoFolder.Infrastructure.Files.FileInfoWrapper;
using IFile = PhotoFolder.Core.Dto.Services.IFile;

namespace PhotoFolder.Infrastructure.Photos
{
    public class PhotoDirectory : IPhotoDirectory
    {
        private readonly DbContextOptions<AppDbContext> _dbOptions;
        private readonly IFileSystem _fileSystem;
        private readonly TemplateString _photoFilenameTemplate;
        private readonly string _rootDirectory;

        public PhotoDirectory(IFileSystem fileSystem, string rootDirectory, PhotoDirectoryConfig config,
            DbContextOptions<AppDbContext> dbOptions)
        {
            _fileSystem = fileSystem;
            _rootDirectory = rootDirectory;

            _photoFilenameTemplate = TemplateString.Parse(config.TemplatePath);
            _dbOptions = dbOptions;
        }

        public IEqualityComparer<string> PathComparer => StringComparer.OrdinalIgnoreCase;

        public IEnumerable<IFile> EnumerateFiles()
        {
            return _fileSystem.DirectoryInfo.FromDirectoryName(_rootDirectory)
                .EnumerateFiles("*", SearchOption.AllDirectories)
                .Where(x => x.Name != PhotoFolderConsts.ConfigFileName && !x.Attributes.HasFlag(FileAttributes.Hidden))
                .Select(x => new FileInfoWrapper(x, _rootDirectory));
        }

        public IFile? GetFile(string filename)
        {
            var fileInfo = _fileSystem.FileInfo.FromFileName(_fileSystem.Path.Combine(_rootDirectory, filename));
            if (!fileInfo.Exists) return null;

            return new FileInfoWrapper(fileInfo, _rootDirectory);
        }

        public bool IsFileInDirectory(FileInformation file) => file.IsRelativeFilename || file.Filename.StartsWith(_rootDirectory);

        public string GetFileDirectoryRegexPattern(FileInformation fileInformation)
        {
            var values = FilePlaceholderFiller.GetPlaceholders(
                _photoFilenameTemplate.Fragments.OfType<PlaceholderFragment>().Select(x => x.Value), fileInformation);
            var nonNullValues = values.Where(x => x.Value != null).ToDictionary(x => x.Key, x => x.Value!);

            var path = _photoFilenameTemplate.ToString(nonNullValues);
            return TemplateString.Parse(_fileSystem.Path.GetDirectoryName(path)).ToRegexPattern();
        }

        public string GetFilenameRegexPattern(FileInformation fileInformation)
        {
            var values = FilePlaceholderFiller.GetPlaceholders(
                _photoFilenameTemplate.Fragments.OfType<PlaceholderFragment>().Select(x => x.Value), fileInformation);
            var nonNullValues = values.Where(x => x.Value != null).ToDictionary(x => x.Key, x => x.Value!);

            var path = _photoFilenameTemplate.ToString(nonNullValues);
            return TemplateString.Parse(path).ToRegexPattern();
        }

        public string GetRecommendedPath(FileInformation fileInformation)
        {
            var values = FilePlaceholderFiller.GetPlaceholders(
                _photoFilenameTemplate.Fragments.OfType<PlaceholderFragment>().Select(x => x.Value), fileInformation);
            var path = _photoFilenameTemplate.ToString(values.ToDictionary(x => x.Key, x => x.Value ?? string.Empty));

            var dirSeparators = new[]
            {
                _fileSystem.Path.DirectorySeparatorChar, _fileSystem.Path.AltDirectorySeparatorChar
            };
            return PathUtilities.PatchPathParts(path, dirSeparators, PathUtilities.TrimChars(' ', '-'),
                PathUtilities.RemoveInvalidChars(_fileSystem.Path.GetInvalidFileNameChars()));
        }

        public IPhotoDirectoryDataContext GetDataContext() => new PhotoDirectoryDataContext(GetAppDbContext());

        public AppDbContext GetAppDbContext() => new AppDbContext(_dbOptions);
    }
}