using Microsoft.Extensions.DependencyInjection;
using PhotoFolder.Application.Shared;
using PhotoFolder.Application.Utilities;
using PhotoFolder.Core.Domain.Entities;
using PhotoFolder.Core.Dto;
using PhotoFolder.Core.Dto.Services;
using PhotoFolder.Core.Dto.UseCaseRequests;
using PhotoFolder.Core.Interfaces.Gateways;
using PhotoFolder.Core.Interfaces.UseCases;
using PhotoFolder.Core.Specifications.FileInformation;
using PhotoFolder.Core.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;

namespace PhotoFolder.Application.Workers
{
    public class SynchronizeIndexWorker
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IEqualityComparer<IFileContentInfo> _fileContentComparer;

        public SynchronizeIndexWorker(IServiceProvider serviceProvider, IEqualityComparer<IFileContentInfo> fileContentComparer)
        {
            State = new SynchronizeIndexState();
            _serviceProvider = serviceProvider;
            _fileContentComparer = fileContentComparer;
        }

        public SynchronizeIndexState State { get; set; }

        public async Task Execute(IPhotoDirectory directory)
        {
            var repository = directory.GetFileRepository();
            var operationsRepository = directory.GetOperationRepository();

            State.Status = SynchronizeIndexStatus.Scanning;

            // get all files from the repository
            var indexedFiles = await repository.GetAllBySpecs(new IncludeFileLocationsSpec());
            var indexedFileInfos = indexedFiles.SelectMany(x => x.ToFileInfos());

            // get all files from the actual directory
            var localFiles = directory.EnumerateFiles().ToList();

            State.Status = SynchronizeIndexStatus.Synchronizing;

            // get changes
            var (newFiles, removedFiles) = CollectionDiff.Create(indexedFileInfos, localFiles,
                new FileInfoComparer());

            // remove files from index
            foreach (var removedFile in removedFiles)
            {
                var action = _serviceProvider.GetRequiredService<IRemoveFileFromIndexUseCase>();
                await action.Handle(new RemoveFileFromIndexRequest(removedFile.Filename, directory));

                if (action.HasError)
                    State.Errors.Add(removedFile.Filename, action.Error!);
            }

            IImmutableList<FileInformation> removedFilesInformation = removedFiles
                .Select(x => indexedFiles.First(y => y.GetFileByFilename(x.Filename) != null).ToFileInformation(x.Filename))
                .ToImmutableList();

            State.Status = SynchronizeIndexStatus.IndexingNewFiles;
            // add files to index
            for (int i = 0; i < newFiles.Count; i++)
            {
                State.Progress = (double) i / newFiles.Count;

                var newFile = newFiles[i];

                var action = _serviceProvider.GetRequiredService<IAddFileToIndexUseCase>();
                var response = await action.Handle(new AddFileToIndexRequest(newFile.Filename, directory));

                if (action.HasError)
                {
                    State.Errors.Add(newFile.Filename, action.Error!);
                    continue;
                }

                var (indexedFile, fileLocation) = response!;

                FileOperation fileOperation;
                (fileOperation, removedFilesInformation) = GetFileOperation(removedFilesInformation, indexedFile, fileLocation,
                    directory.PathComparer, _fileContentComparer);

                await operationsRepository.Add(fileOperation);
            }

            foreach (var removedFile in removedFilesInformation)
            {
                await operationsRepository.Add(FileOperation.FileRemoved(removedFile));
            }
        }

        private static (FileOperation op, IImmutableList<FileInformation> removedFiles) GetFileOperation(
            IImmutableList<FileInformation> removedFiles, IndexedFile indexedFile, FileLocation fileLocation,
            IEqualityComparer<string> pathComparer, IEqualityComparer<IFileContentInfo> fileContentComparer)
        {
            // get operation
            if (removedFiles.Any())
            {
                var removedFile = removedFiles.FirstOrDefault(x => fileContentComparer.Equals(x, indexedFile));

                if (removedFile != null)
                {
                    var newRemovedFiles = removedFiles.Remove(removedFile);

                    if (pathComparer.Equals(fileLocation.Filename, removedFile.Filename))
                        return (FileOperation.FileChanged(fileLocation, removedFile), newRemovedFiles);
                    else
                        return (FileOperation.FileMoved(fileLocation, removedFile), newRemovedFiles);
                }
            }

            return (FileOperation.NewFile(fileLocation), removedFiles);
        }
    }

    public class SynchronizeIndexState : PropertyChangedBase
    {
        private SynchronizeIndexStatus _status;
        private double _progress;

        public SynchronizeIndexStatus Status
        {
            get => _status;
            internal set => SetProperty(ref _status, value);
        }

        public double Progress
        {
            get => _progress;
            internal set => SetProperty(ref _progress, value);
        }

        public Dictionary<string, Error> Errors { get; } = new Dictionary<string, Error>();
    }

    public enum SynchronizeIndexStatus
    {
        Scanning,
        Synchronizing,
        IndexingNewFiles
    }
}
