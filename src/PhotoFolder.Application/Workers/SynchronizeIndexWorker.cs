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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using PhotoFolder.Core.Domain;
using PhotoFolder.Core.Interfaces.Gateways;
using PhotoFolder.Core.Interfaces;
using PhotoFolder.Core.Dto.UseCaseResponses;
using Microsoft.Extensions.DependencyInjection;

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

        public Task<SynchronizeIndexResponse> Execute(SynchronizeIndexRequest request, CancellationToken cancellationToken = default)
        {
            return Task.Run(() => InternalExecute(request, cancellationToken));
        }

        public async Task<SynchronizeIndexResponse> InternalExecute(SynchronizeIndexRequest request, CancellationToken cancellationToken = default)
        {
            var directory = request.Directory;
            using var dataContext = directory.GetDataContext();
            var operations = new ConcurrentBag<FileOperation>();

            State.Status = SynchronizeIndexStatus.Scanning;

            // get all files from the repository
            var indexedFiles = await dataContext.FileRepository.GetAllBySpecs(new IncludeFileLocationsSpec());
            var indexedFileInfos = indexedFiles.SelectMany(x => x.ToFileInfos(directory));

            // get all files from the actual directory
            var localFiles = directory.EnumerateFiles().WithCancellation(cancellationToken).ToList();

            State.Status = SynchronizeIndexStatus.Synchronizing;

            // get changes
            var (newFiles, removedFiles) = CollectionDiff.Create(indexedFileInfos, localFiles, new FileInfoComparer());

            // files that are completely removed from the directory
            var completelyRemovedFiles = new List<IFileInfo>();

            // remove files from index
            foreach (var removedFile in removedFiles)
            {
                var action = _serviceProvider.GetRequiredService<IRemoveFileFromIndexUseCase>();
                var response = await action.Handle(new RemoveFileFromIndexRequest(removedFile.RelativeFilename!, directory));

                if (action.HasError)
                    State.Errors.Add(removedFile.RelativeFilename!, action.Error!);

                if (response!.IsCompletelyRemoved)
                    completelyRemovedFiles.Add(removedFile);
            }

            IImmutableList<FileInformation> removedFileInformation = removedFiles
                .Select(x => GetFileInformationFromPath(x.RelativeFilename!, indexedFiles, directory))
                .ToImmutableList();

            var formerlyDeletedFiles = directory.DeletedFiles.Files;
            var deletedFilesLock = new object();

            State.Status = SynchronizeIndexStatus.IndexingNewFiles;
            State.TotalFiles = newFiles.Count;

            var processedFilesCount = 0;
            var removedFilesLock = new object();
            var stateLock = new object();

            await TaskCombinators.ThrottledAsync(newFiles, async (newFile, _) =>
            {
                var (action, response) = await IndexFile(newFile.Filename, directory);

                if (action.HasError)
                {
                    lock (stateLock)
                    {
                        State.Errors.Add(newFile.Filename, action.Error!);
                    }
                    return;
                }

                var (indexedFile, fileLocation) = response!;

                // remove from formerly deleted files
                if (formerlyDeletedFiles.ContainsKey(indexedFile.Hash))
                    lock (deletedFilesLock)
                    {
                        formerlyDeletedFiles = formerlyDeletedFiles.Remove(indexedFile.Hash);
                    }

                FileOperation fileOperation;

                // get operation
                var removedFile = removedFileInformation.FirstOrDefault(x => _fileContentComparer.Equals(x, indexedFile));
                if (removedFile != null)
                {
                    lock (removedFilesLock)
                    {
                        removedFileInformation = removedFileInformation.Remove(removedFile);
                    }

                    if (directory.PathComparer.Equals(fileLocation.RelativeFilename, removedFile.RelativeFilename!))
                        fileOperation = FileOperation.FileChanged(fileLocation, ToFileReference(removedFile));
                    else
                        fileOperation = FileOperation.FileMoved(fileLocation, ToFileReference(removedFile));
                }
                else
                    fileOperation = FileOperation.NewFile(fileLocation);

                using (var context = directory.GetDataContext())
                {
                    await context.OperationRepository.Add(fileOperation);
                    operations.Add(fileOperation);
                }

                var processedFiles = Interlocked.Increment(ref processedFilesCount);
                State.ProcessedFiles = processedFiles;
                State.Progress = (double)processedFiles / newFiles.Count;
            }, CancellationToken.None); // do not use cancellation token here as a cancellation would destroy all move/change operations as all files were already removed

            foreach (var removedFile in removedFileInformation)
            {
                var operation = FileOperation.FileRemoved(ToFileReference(removedFile));
                await dataContext.OperationRepository.Add(operation);

                operations.Add(operation);

                // add the file to deleted files, if it was completely removed from index
                // WARN: if a file changes, the previous file is not marked as deleted. Idk if that is actually desired
                if (completelyRemovedFiles.Any(x => x.Filename == removedFile.Filename))
                {
                    formerlyDeletedFiles = formerlyDeletedFiles.Add(removedFile.Hash.ToString(), new DeletedFileInfo(removedFile.RelativeFilename!, removedFile.Length, removedFile.Hash, removedFile.PhotoProperties, removedFile.FileCreatedOn, DateTimeOffset.UtcNow));
                }
            }

            await directory.DeletedFiles.Update(formerlyDeletedFiles);
            return new SynchronizeIndexResponse(operations.ToList());
        }

        private async Task<(IUseCaseErrors, AddFileToIndexResponse?)> IndexFile(string filename, IPhotoDirectory photoDirectory)
        {
            var action = _serviceProvider.GetRequiredService<IAddFileToIndexUseCase>();

            AddFileToIndexResponse? response;
            try
            {
                response = await action.Handle(new AddFileToIndexRequest(filename, photoDirectory));
            }
            catch (Exception e)
            {
                if (e.GetType().Name.Equals("DbUpdateException"))
                {
                    response = await action.Handle(new AddFileToIndexRequest(filename, photoDirectory));
                }
                else throw;
            }

            return (action, response);
        }
        private FileInformation GetFileInformationFromPath(string relativeFilename, IReadOnlyList<IndexedFile> indexedFiles, IPhotoDirectory photoDirectory)
        {
            return indexedFiles.First(y => y.HasPath(relativeFilename)).ToFileInformation(relativeFilename, photoDirectory);
        }

        private static IFileReference ToFileReference(FileInformation fileInformation)
        {
            if (fileInformation.RelativeFilename == null)
                throw new ArgumentException("Cannot convert file information to reference when it's not in photo directory.");

            return new FileReference(fileInformation.Hash.ToString(), fileInformation.RelativeFilename);
        }
    }
}
