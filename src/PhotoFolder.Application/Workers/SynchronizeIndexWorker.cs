using Microsoft.Extensions.DependencyInjection;
using PhotoFolder.Application.Shared;
using PhotoFolder.Application.Utilities;
using PhotoFolder.Core.Domain.Entities;
using PhotoFolder.Core.Dto;
using PhotoFolder.Core.Dto.UseCaseRequests;
using PhotoFolder.Core.Interfaces.Gateways;
using PhotoFolder.Core.Interfaces.Services;
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

        public SynchronizeIndexWorker(IServiceProvider serviceProvider)
        {
            State = new SynchronizeIndexState();
            _serviceProvider = serviceProvider;
        }

        public SynchronizeIndexState State { get; set; }

        public async Task Execute(IPhotoDirectory directory)
        {
            var repository = directory.GetFileRepository();

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

            var removedIndexedFiles = removedFiles
                .Select(x => indexedFiles.First(y => y.GetFileByFilename(x.Filename) != null).ToFileInformation(x.Filename))
                .ToImmutableList();

            State.Status = SynchronizeIndexStatus.IndexingNewFiles;
            // add files to index
            for (int i = 0; i < newFiles.Count; i++)
            {
                State.Progress = (double) i / newFiles.Count;

                var newFile = newFiles[i];

                var action = _serviceProvider.GetRequiredService<IAddFileToIndexUseCase>();
                await action.Handle(new AddFileToIndexRequest(
                    newFile.Filename, directory, removedIndexedFiles));

                if (action.HasError)
                    State.Errors.Add(newFile.Filename, action.Error!);
            }
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
