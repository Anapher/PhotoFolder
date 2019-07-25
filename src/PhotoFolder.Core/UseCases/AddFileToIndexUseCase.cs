using PhotoFolder.Core.Domain.Entities;
using PhotoFolder.Core.Dto.Services;
using PhotoFolder.Core.Dto.UseCaseRequests;
using PhotoFolder.Core.Dto.UseCaseResponses;
using PhotoFolder.Core.Errors;
using PhotoFolder.Core.Interfaces;
using PhotoFolder.Core.Interfaces.Gateways.Repositories;
using PhotoFolder.Core.Interfaces.Services;
using PhotoFolder.Core.Interfaces.UseCases;
using PhotoFolder.Core.Specifications.FileInformation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PhotoFolder.Core.UseCases
{
    public class AddFileToIndexUseCase : UseCaseStatus<AddFileToIndexResponse>, IAddFileToIndexUseCase
    {
        private readonly IFileInformationLoader _fileInformationLoader;
        private readonly IFileHasher _fileHasher;
        private readonly IEqualityComparer<IFileContentInfo> _fileContentComparer;

        public AddFileToIndexUseCase(IFileInformationLoader fileInformationLoader, IFileHasher fileHasher,
            IEqualityComparer<IFileContentInfo> fileContentComparer)
        {
            _fileInformationLoader = fileInformationLoader;
            _fileHasher = fileHasher;
            _fileContentComparer = fileContentComparer;
        }

        public async Task<AddFileToIndexResponse?> Handle(AddFileToIndexRequest message)
        {
            var directory = message.Directory;

            var file = directory.GetFile(message.Filename);
            if (file == null)
                return ReturnError(new FileNotFoundError(message.Filename));

            var repository = directory.GetFileRepository();

            // check if the file already exists
            var existingFile = await repository.FirstOrDefaultBySpecs(new FindByFilenameSpec(message.Filename));
            if (existingFile != null)
            {
                return ReturnError(new InvalidOperationError("The file was already indexed. Please remove it first",
                    ErrorCode.FileAlreadyIndexed));
            }

            // get file information
            IndexedFile indexedFile;
            bool isIndexedFileInDb;
            try
            {
                (indexedFile, isIndexedFileInDb) = await GetFileInformation(file, repository);
            }
            catch (Exception)
            {
                // if the file was removed
                if (directory.GetFile(message.Filename) == null)
                    return ReturnError(new FileNotFoundError(message.Filename));

                throw;
            }

            var fileLocation = new FileLocation(file.Filename, indexedFile.Hash,
                file.CreatedOn, file.ModifiedOn);
            indexedFile.AddLocation(fileLocation);

            // get operation
            FileOperation? fileOperation = null;
            if (message.RemovedFiles.Any())
            {
                var removedFile = message
                    .RemovedFiles
                    .FirstOrDefault(x => _fileContentComparer.Equals(x, indexedFile));

                if (removedFile != null)
                {
                    if (directory.PathComparer.Equals(removedFile.Filename, removedFile.Filename))
                        fileOperation = FileOperation.FileChanged(fileLocation, removedFile);
                    else
                        fileOperation = FileOperation.FileMoved(fileLocation, removedFile);
                }
            }

            if (fileOperation == null)
                fileOperation = FileOperation.NewFile(fileLocation);

            if (isIndexedFileInDb)
                await repository.Update(indexedFile);
            else
                await repository.Add(indexedFile);

            var operationsRepository = directory.GetOperationRepository();
            await operationsRepository.Add(fileOperation);

            return new AddFileToIndexResponse();
        }

        private Hash ComputeHash(IFile file)
        {
            using (var stream = file.OpenRead())
            {
                return _fileHasher.ComputeHash(stream);
            }
        }

        private async Task<(IndexedFile, bool fromBb)> GetFileInformation(IFile file, IIndexedFileRepository repository)
        {
            var fileHash = ComputeHash(file);
            var indexedFile = await repository.FirstOrDefaultBySpecs(new FindByFileHashSpec(fileHash),
                new IncludeFileLocationsSpec());

            if (indexedFile != null)
                return (indexedFile, true);

            var info = await _fileInformationLoader.Load(file);

            indexedFile = new IndexedFile(Hash.Parse(info.Hash), info.Length, info.FileCreatedOn, info.PhotoProperties);
            return (indexedFile, false);
        }
    }
}
