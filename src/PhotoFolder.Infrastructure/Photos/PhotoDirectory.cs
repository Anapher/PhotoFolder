using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using PhotoFolder.Core.Domain;
using PhotoFolder.Core.Domain.Entities;
using PhotoFolder.Core.Domain.Template;
using PhotoFolder.Core.Dto.Services;
using PhotoFolder.Core.Interfaces.Gateways;
using PhotoFolder.Infrastructure.Consts;
using PhotoFolder.Infrastructure.Data;
using PhotoFolder.Infrastructure.Utilities;
using FileInfoWrapper = PhotoFolder.Infrastructure.Files.FileInfoWrapper;
using IFile = PhotoFolder.Core.Dto.Services.IFile;
using IFileInfo = System.IO.Abstractions.IFileInfo;

namespace PhotoFolder.Infrastructure.Photos
{
    // we are using internally forward slashes (/) because they don't escape
    public class PhotoDirectory : IPhotoDirectory
    {
        private readonly DbContextOptions<AppDbContext> _dbOptions;
        private readonly IFileSystem _fileSystem;
        private readonly TemplateString _photoFilenameTemplate;
        private readonly string _rootDirectory;

        public PhotoDirectory(IFileSystem fileSystem, string rootDirectory, PhotoDirectoryConfig config,
            DbContextOptions<AppDbContext> dbOptions, IDeletedFiles deletedFiles)
        {
            _fileSystem = fileSystem;
            _rootDirectory = rootDirectory.ToForwardSlashes().TrimEnd('/');

            _photoFilenameTemplate = TemplateString.Parse(config.TemplatePath.ToForwardSlashes());
            _dbOptions = dbOptions;
            DeletedFiles = deletedFiles;
        }

        public IEqualityComparer<string> PathComparer => StringComparer.OrdinalIgnoreCase;
        public IDeletedFiles DeletedFiles { get; }

        public IEnumerable<IFile> EnumerateFiles()
        {
            return _fileSystem.DirectoryInfo.FromDirectoryName(_rootDirectory)
                .EnumerateFiles("*", SearchOption.AllDirectories)
                .Where(x => x.Name != PhotoFolderConsts.ConfigFileName && !x.Attributes.HasFlag(FileAttributes.Hidden))
                .Select(ToWrapper);
        }

        public IFile? GetFile(string filename)
        {
            var fileInfo = _fileSystem.FileInfo.FromFileName(_fileSystem.Path.Combine(_rootDirectory, filename));
            if (!fileInfo.Exists) return null;

            return ToWrapper(fileInfo);
        }

        private FileInfoWrapper ToWrapper(IFileInfo fileInfo)
        {
            var filename = fileInfo.FullName.ToForwardSlashes();
            if (filename.StartsWith(_rootDirectory))
                return new FileInfoWrapper(fileInfo, filename.Substring(_rootDirectory.Length + 1)); // + 1 for the slash

            return new FileInfoWrapper(fileInfo, null);
        }

        public TemplateString GetFileDirectoryTemplate(FileInformation fileInformation)
        {
            var values = FilePlaceholderFiller.GetPlaceholders(
                _photoFilenameTemplate.Fragments.OfType<PlaceholderFragment>().Select(x => x.Value), fileInformation);
            var nonNullValues = values.Where(x => x.Value != null).ToDictionary(x => x.Key, x => x.Value!);

            var path = _photoFilenameTemplate.ToString(nonNullValues);
            return TemplateString.Parse(_fileSystem.Path.GetDirectoryName(path).ToForwardSlashes());
        }

        public TemplateString GetFilenameTemplate(FileInformation fileInformation)
        {
            var values = FilePlaceholderFiller.GetPlaceholders(
                _photoFilenameTemplate.Fragments.OfType<PlaceholderFragment>().Select(x => x.Value), fileInformation);
            var nonNullValues = values.Where(x => x.Value != null).ToDictionary(x => x.Key, x => x.Value!);

            var path = _photoFilenameTemplate.ToString(nonNullValues).ToForwardSlashes();
            return TemplateString.Parse(path);
        }

        public string ClearPath(string path)
        {
            var dirSeparators = new[]
            {
                _fileSystem.Path.DirectorySeparatorChar, _fileSystem.Path.AltDirectorySeparatorChar
            };
            return PathUtilities.PatchPathParts(path, dirSeparators, PathUtilities.TrimChars(' ', '-'),
                PathUtilities.RemoveInvalidChars(_fileSystem.Path.GetInvalidFileNameChars())).ToForwardSlashes();
        }

        public string GetRecommendedPath(FileInformation fileInformation)
        {
            var values = FilePlaceholderFiller.GetPlaceholders(
                _photoFilenameTemplate.Fragments.OfType<PlaceholderFragment>().Select(x => x.Value), fileInformation);
            var path = _photoFilenameTemplate.ToString(values.ToDictionary(x => x.Key, x => x.Value ?? string.Empty));

            return ClearPath(path);
        }

        public FileInformation ToFileInformation(IndexedFile indexedFile, FileLocation fileLocation)
        {
            if (indexedFile.Files.All(x => x != fileLocation))
                throw new ArgumentException("Can only get file information if the location belongs to the indexed file.", nameof(fileLocation));

            return new FileInformation(_fileSystem.Path.Combine(_rootDirectory, fileLocation.RelativeFilename).ToForwardSlashes(), fileLocation.CreatedOn,
                fileLocation.ModifiedOn, Hash.Parse(indexedFile.Hash), indexedFile.Length, indexedFile.FileCreatedOn, indexedFile.PhotoProperties,
                fileLocation.RelativeFilename);
        }

        public IPhotoDirectoryDataContext GetDataContext() => new PhotoDirectoryDataContext(GetAppDbContext());

        public AppDbContext GetAppDbContext() => new AppDbContext(_dbOptions);

        public void UpdateDeletedFiles(IReadOnlyDictionary<string, DeletedFileInfo> deletedFiles)
        {

        }
    }
}