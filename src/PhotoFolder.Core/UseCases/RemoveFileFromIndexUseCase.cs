using PhotoFolder.Core.Domain.Entities;
using PhotoFolder.Core.Dto.UseCaseRequests;
using PhotoFolder.Core.Dto.UseCaseResponses;
using PhotoFolder.Core.Errors;
using PhotoFolder.Core.Interfaces;
using PhotoFolder.Core.Interfaces.UseCases;
using PhotoFolder.Core.Specifications.FileInformation;
using System.Linq;
using System.Threading.Tasks;

namespace PhotoFolder.Core.UseCases
{
    public class RemoveFileFromIndexUseCase : UseCaseStatus<RemoveFileFromIndexResponse>, IRemoveFileFromIndexUseCase
    {
        public async Task<RemoveFileFromIndexResponse?> Handle(RemoveFileFromIndexRequest message)
        {
            var directory = message.PhotoDirectory;
            var repository = directory.GetFileRepository();

            // check if the file already exists
            var existingFile = await repository.FirstOrDefaultBySpecs(new FindByFilenameSpec(message.Filename),
                new IncludeFileLocationsSpec());
            if (existingFile == null)
            {
                return ReturnError(new InvalidOperationError("The file is not indexed.",
                    ErrorCode.FileNotIndexed));
            }

            var indexedFile = existingFile.Files.First(x => directory.PathComparer.Equals(
                message.Filename, message.Filename));
            existingFile.RemoveLocation(indexedFile.Filename);

            if (existingFile.Files.Any())
            {
                await repository.Update(existingFile);
            }
            else
            {
                await repository.Delete(existingFile);
            }

            var operationsRepo = directory.GetOperationRepository();
            await operationsRepo.Add(FileOperation.FileRemoved(indexedFile));

            return new RemoveFileFromIndexResponse();
        }
    }
}
