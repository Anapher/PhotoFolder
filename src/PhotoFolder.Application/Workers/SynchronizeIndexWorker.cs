using Microsoft.Extensions.DependencyInjection;
using PhotoFolder.Application.Dto.WorkerRequests;
using PhotoFolder.Application.Dto.WorkerResponses;
using PhotoFolder.Application.Dto.WorkerStates;
using PhotoFolder.Application.Interfaces.Workers;
using PhotoFolder.Application.Utilities;
using PhotoFolder.Core.Domain.Entities;
using PhotoFolder.Core.Dto.Services;
using PhotoFolder.Core.Dto.UseCaseRequests;
using PhotoFolder.Core.Interfaces.UseCases;
using PhotoFolder.Core.Specifications.FileInformation;
using PhotoFolder.Core.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PhotoFolder.Application.Workers
{
    public class SynchronizeIndexWorker : ISynchronizeIndexWorker
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IEqualityComparer<IFileContentInfo> _fileContentComparer;

        public SynchronizeIndexWorker(IServiceProvider serviceProvider, IEqualityComparer<IFileContentInfo> fileContentComparer)
        {
            State = new SynchronizeIndexState();
            _serviceProvider = serviceProvider;
            _fileContentComparer = fileContentComparer;
        }

        public SynchronizeIndexState State { get; }

        public async Task<SynchronizeIndexResponse> Execute(SynchronizeIndexRequest request, CancellationToken cancellationToken = default)
        {
            var directory = request.Directory;

            using (var dataContext = directory.GetDataContext())
            {
                State.Status = SynchronizeIndexStatus.Scanning;

                // get all files from the repository
                var indexedFiles = await dataContext.FileRepository.GetAllBySpecs(new IncludeFileLocationsSpec());
                var indexedFileInfos = indexedFiles.SelectMany(x => x.ToFileInfos());

                // get all files from the actual directory
                var localFiles = directory.EnumerateFiles().WithCancellation(cancellationToken).ToList();

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
                    State.Progress = (double)i / newFiles.Count;

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

                    await dataContext.OperationRepository.Add(fileOperation);
                }

                foreach (var removedFile in removedFilesInformation)
                {
                    await dataContext.OperationRepository.Add(FileOperation.FileRemoved(removedFile));
                }

            }

            return new SynchronizeIndexResponse();
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
}
