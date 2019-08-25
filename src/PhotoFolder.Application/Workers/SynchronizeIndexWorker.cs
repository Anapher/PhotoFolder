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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using PhotoFolder.Core.Domain;
using PhotoFolder.Core.Dto.UseCaseResponses;

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

            // remove files from index
            foreach (var removedFile in removedFiles)
            {
                var action = _serviceProvider.GetRequiredService<IRemoveFileFromIndexUseCase>();
                await action.Handle(new RemoveFileFromIndexRequest(removedFile.RelativeFilename!, directory));

                if (action.HasError)
                    State.Errors.Add(removedFile.RelativeFilename!, action.Error!);
            }

            IImmutableList<FileInformation> removedFilesInformation = removedFiles.Where(x => x.RelativeFilename != null).Select(x =>
                indexedFiles.First(y => y.GetFileByFilename(x.RelativeFilename!) != null).ToFileInformation(x.RelativeFilename!, directory)).ToImmutableList();

            State.Status = SynchronizeIndexStatus.IndexingNewFiles;
            State.TotalFiles = newFiles.Count;

            var processedFilesCount = 0;

            void FileProcessed()
            {
                var processedFiles = Interlocked.Increment(ref processedFilesCount);
                State.ProcessedFiles = processedFiles;
                State.Progress = (double) processedFiles / newFiles.Count;
            }

            var removedFilesLock = new object();

            await TaskCombinators.ThrottledAsync(newFiles, async (newFile, _) =>
            {
                var action = _serviceProvider.GetRequiredService<IAddFileToIndexUseCase>();

                AddFileToIndexResponse? response;
                try
                {
                    response = await action.Handle(new AddFileToIndexRequest(newFile.Filename, directory));
                }
                catch (Exception e)
                {
                    if (e.GetType().Name.Equals("DbUpdateException"))
                    {
                        response = await action.Handle(new AddFileToIndexRequest(newFile.Filename, directory));
                    }
                    else throw;
                }

                if (action.HasError)
                {
                    State.Errors.Add(newFile.Filename, action.Error!);
                    return;
                }

                var (indexedFile, fileLocation) = response!;

                FileOperation fileOperation;

                // get operation
                if (removedFiles.Any())
                {
                    var removedFile = removedFilesInformation.FirstOrDefault(x => _fileContentComparer.Equals(x, indexedFile));

                    if (removedFile != null)
                    {
                        lock (removedFilesLock)
                        {
                            removedFilesInformation = removedFilesInformation.Remove(removedFile);
                        }

                        if (directory.PathComparer.Equals(fileLocation.RelativeFilename, removedFile.RelativeFilename!))
                            fileOperation = FileOperation.FileChanged(fileLocation, ToFileReference(removedFile));
                        else
                            fileOperation = FileOperation.FileMoved(fileLocation, ToFileReference(removedFile));
                    }
                    else
                        fileOperation = FileOperation.NewFile(fileLocation);
                }
                else
                {
                    fileOperation = FileOperation.NewFile(fileLocation);
                }

                using (var context = directory.GetDataContext())
                {
                    await context.OperationRepository.Add(fileOperation);
                    operations.Add(fileOperation);
                }

                FileProcessed();
            }, CancellationToken.None);

            foreach (var removedFile in removedFilesInformation)
            {
                var operation = FileOperation.FileRemoved(ToFileReference(removedFile));
                await dataContext.OperationRepository.Add(operation);

                operations.Add(operation);
            }

            return new SynchronizeIndexResponse(operations.ToList());
        }

        private static IFileReference ToFileReference(FileInformation fileInformation)
        {
            if (fileInformation.RelativeFilename == null)
                throw new ArgumentException("Cannot convert file information to reference when it's not in photo directory.");

            return new FileReference(fileInformation.Hash.ToString(), fileInformation.RelativeFilename);
        }
    }
}
