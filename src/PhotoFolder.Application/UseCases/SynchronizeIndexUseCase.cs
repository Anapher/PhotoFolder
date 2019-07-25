using Microsoft.Extensions.DependencyInjection;
using PhotoFolder.Application.Utilities;
using PhotoFolder.Core.Domain.Entities;
using PhotoFolder.Core.Dto;
using PhotoFolder.Core.Dto.Services;
using PhotoFolder.Core.Dto.UseCaseRequests;
using PhotoFolder.Core.Interfaces.Gateways;
using PhotoFolder.Core.Interfaces.Services;
using PhotoFolder.Core.Interfaces.UseCases;
using PhotoFolder.Core.Specifications.FileInformation;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;

namespace PhotoFolder.Application.UseCases
{
    public class SynchronizeIndexUseCase
    {
        private readonly IFileInformationLoader _filePropertiesLoader;
        private readonly IEqualityComparer<FileInformation> _fileInformationComparer;
        private readonly IServiceProvider _serviceProvider;

        public SynchronizeIndexUseCase(IEqualityComparer<FileInformation> fileInformationComparer,
            IFileInformationLoader filePropertiesLoader, IServiceProvider serviceProvider)
        {
            _filePropertiesLoader = filePropertiesLoader;

            State = new SynchronizeIndexState();
            _fileInformationComparer = fileInformationComparer;
            _serviceProvider = serviceProvider;
        }

        public SynchronizeIndexState State { get; set; }

        public async Task Execute(IPhotoDirectory directory)
        {
            var repository = directory.GetFileRepository();

            State.Status = SynchronizeIndexStatus.Scanning;

            // get all files from the repository
            var fileInformation = await repository.GetAllBySpecs(new IncludeFileLocationsSpec());
            var indexedFiles = fileInformation.SelectMany(ToFileInfos);

            // get all files from the actual directory
            var localFiles = directory.EnumerateFiles().ToList();

            State.Status = SynchronizeIndexStatus.Synchronizing;

            // get changes
            var (newFiles, removedFiles) = CollectionDiff.Create(indexedFiles, localFiles,
                directory.FileInfoComparer);

            // remove files from index
            foreach (var removedFile in removedFiles)
            {
                var action = _serviceProvider.GetRequiredService<IRemoveFileFromIndexUseCase>();
                await action.Handle(new RemoveFileFromIndexRequest(removedFile.Filename, directory));

                if (action.HasError)
                    State.Errors.Add(removedFile.Filename, action.Error!);
            }

            var removedFilesInformation = removedFiles
                .Select(x => fileInformation.First(y => y.GetFileByFilename(x.Filename) != null))
                .ToImmutableList();

            // add files to index
            for (int i = 0; i < newFiles.Count; i++)
            {
                State.Progress = (double) i / newFiles.Count;

                var newFile = newFiles[i];

                var action = _serviceProvider.GetRequiredService<IAddFileToIndexUseCase>();
                await action.Handle(new AddFileToIndexRequest(
                    newFile.Filename, directory, removedFilesInformation));

                if (action.HasError)
                    State.Errors.Add(newFile.Filename, action.Error!);
            }
        }

        private static IEnumerable<IFileInfo> ToFileInfos(FileInformation file)
        {
            return file.Files.Select(x => new BasicFileInfo(x.Filename, file.Length, x.CreatedOn, x.ModifiedOn));
        }
    }

    public class BasicFileInfo : IFileInfo
    {
        public BasicFileInfo(string filename, long length, DateTimeOffset createdOn, DateTimeOffset modifiedOn)
        {
            Filename = filename;
            Length = length;
            CreatedOn = createdOn;
            ModifiedOn = modifiedOn;
        }

        public string Filename { get; }
        public long Length { get; }

        public DateTimeOffset CreatedOn { get; }
        public DateTimeOffset ModifiedOn { get; }
    }

    public class SynchronizeIndexState
    {
        public SynchronizeIndexStatus Status { get; internal set; }
        public double Progress { get; internal set; }

        public Dictionary<string, Error> Errors { get; } = new Dictionary<string, Error>();
    }

    public enum SynchronizeIndexStatus
    {
        Scanning,
        Synchronizing
    }
}
