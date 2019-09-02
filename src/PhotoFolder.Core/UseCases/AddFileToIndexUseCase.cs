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
using System.Threading.Tasks;
using PhotoFolder.Core.Domain;

namespace PhotoFolder.Core.UseCases
{
    public class AddFileToIndexUseCase : UseCaseStatus<AddFileToIndexResponse>, IAddFileToIndexUseCase
    {
        private readonly IFileInformationLoader _fileInformationLoader;
        private readonly IFileHasher _fileHasher;

        public AddFileToIndexUseCase(IFileInformationLoader fileInformationLoader, IFileHasher fileHasher)
        {
            _fileInformationLoader = fileInformationLoader;
            _fileHasher = fileHasher;
        }

        public async Task<AddFileToIndexResponse?> Handle(AddFileToIndexRequest message)
        {
            var directory = message.Directory;

            var file = directory.GetFile(message.Filename);
            if (file == null)
                return ReturnError(new FileNotFoundError(message.Filename));

            if (file.RelativeFilename == null)
                return ReturnError(new InvalidOperationError("The file must be in the photo directory", ErrorCode.FileNotInPhotoDirectory));

            await using var dataContext = directory.GetDataContext();

            // check if the file already exists
            var existingFile = await dataContext.FileRepository.FirstOrDefaultBySpecs(new FindByFilenameSpec(file.RelativeFilename));
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
                (indexedFile, isIndexedFileInDb) = await GetFileInformation(file, dataContext.FileRepository);
            }
            catch (Exception)
            {
                // if the file was removed
                if (directory.GetFile(message.Filename) == null)
                    return ReturnError(new FileNotFoundError(message.Filename));

                throw;
            }

            var fileLocation = new FileLocation(file.RelativeFilename, indexedFile.Hash,
                file.CreatedOn, file.ModifiedOn);
            indexedFile.AddLocation(fileLocation);

            if (isIndexedFileInDb)
                await dataContext.FileRepository.Update(indexedFile);
            else
                await dataContext.FileRepository.Add(indexedFile);

            return new AddFileToIndexResponse(indexedFile, fileLocation, !isIndexedFileInDb);
        }

        private Hash ComputeHash(IFile file)
        {
            using var stream = file.OpenRead();
            return _fileHasher.ComputeHash(stream);
        }

        private async Task<(IndexedFile, bool fromBb)> GetFileInformation(IFile file, IIndexedFileRepository repository)
        {
            var fileHash = ComputeHash(file);
            var indexedFile = await repository.FirstOrDefaultBySpecs(new FindByFileHashSpec(fileHash),
                new IncludeFileLocationsSpec());

            if (indexedFile != null)
                return (indexedFile, true);

            var info = await _fileInformationLoader.Load(file);

            indexedFile = new IndexedFile(info.Hash, info.Length, info.FileCreatedOn, info.PhotoProperties);
            return (indexedFile, false);
        }
    }
}
