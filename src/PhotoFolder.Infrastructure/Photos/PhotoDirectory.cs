using Microsoft.EntityFrameworkCore;
using PhotoFolder.Core.Domain.Entities;
using PhotoFolder.Core.Interfaces.Gateways;
using PhotoFolder.Core.Interfaces.Gateways.Repositories;
using PhotoFolder.Infrastructure.Consts;
using PhotoFolder.Infrastructure.Data;
using PhotoFolder.Infrastructure.Data.Repositories;
using PhotoFolder.Infrastructure.TemplatePath;
using PhotoFolder.Infrastructure.Utilities;
using System;
using System.Collections.Generic;
using System.IO.Abstractions;
using System.Linq;

namespace PhotoFolder.Infrastructure.Photos
{
    public class PhotoDirectory : IPhotoDirectory
    {
        private readonly IFileSystem _fileSystem;
        private readonly string _rootDirectory;
        private readonly TemplateString _photoFilenameTemplate;
        private readonly DbContextOptions<AppDbContext> _dbOptions;

        public PhotoDirectory(IFileSystem fileSystem, string rootDirectory, PhotoDirectoryConfig config,
            DbContextOptions<AppDbContext> dbOptions)
        {
            _fileSystem = fileSystem;
            _rootDirectory = rootDirectory;

            _photoFilenameTemplate = TemplateString.Parse(config.TemplatePath);
            _dbOptions = dbOptions;
        }

        public IEqualityComparer<string> PathComparer => StringComparer.OrdinalIgnoreCase;

        public IEnumerable<Core.Dto.Services.IFile> EnumerateFiles()
        {
            return _fileSystem
                .DirectoryInfo
                .FromDirectoryName(_rootDirectory)
                .EnumerateFiles("*", System.IO.SearchOption.AllDirectories)
                .Where(x => x.Name != PhotoFolderConsts.ConfigFileName)
                .Select(x => new Files.FileInfoWrapper(x, _rootDirectory));
        }

        public Core.Dto.Services.IFile? GetFile(string filename)
        {
            var fileInfo = _fileSystem.FileInfo.FromFileName(_fileSystem.Path.Combine(_rootDirectory, filename));
            if (!fileInfo.Exists) return null;

            return new Files.FileInfoWrapper(fileInfo, _rootDirectory);
        }

        public string GetFileDirectoryRegexPattern(FileInformation fileInformation)
        {
            var values = FilePlaceholderFiller.GetPlaceholders(_photoFilenameTemplate.Fragments.OfType<PlaceholderFragment>().Select(x => x.Value), fileInformation);
            var path = _photoFilenameTemplate.ToString(values.Where(x => x.Value != null).ToDictionary(x => x.Key, x => x.Value!));

            return TemplateString.Parse(_fileSystem.Path.GetDirectoryName(path)).ToRegexPattern();
        }

        public string GetFilenameRegexPattern(FileInformation fileInformation)
        {
            var values = FilePlaceholderFiller.GetPlaceholders(_photoFilenameTemplate.Fragments.OfType<PlaceholderFragment>().Select(x => x.Value), fileInformation);
            var path = _photoFilenameTemplate.ToString(values.Where(x => x.Value != null).ToDictionary(x => x.Key, x => x.Value!));

            return TemplateString.Parse(_fileSystem.Path.GetFileName(path)).ToRegexPattern();
        }

        public string GetRecommendedPath(FileInformation fileInformation)
        {
            var values = FilePlaceholderFiller.GetPlaceholders(_photoFilenameTemplate.Fragments.OfType<PlaceholderFragment>().Select(x => x.Value), fileInformation);
            var path = _photoFilenameTemplate.ToString(values.ToDictionary(x => x.Key, x => x.Value ?? string.Empty));

            var dirSeparators = new[] { _fileSystem.Path.DirectorySeparatorChar, _fileSystem.Path.AltDirectorySeparatorChar };
            return PathUtilities.PatchPathParts(path, dirSeparators, PathUtilities.TrimChars(' ', '-'),
                PathUtilities.RemoveInvalidChars(_fileSystem.Path.GetInvalidFileNameChars()));
        }

        public AppDbContext GetAppDbContext()
        {
            return new AppDbContext(_dbOptions);
        }

        public IPhotoDirectoryDataContext GetDataContext()
        {
            return new PhotoDirectoryDataContext(GetAppDbContext());
        }
    }
}
