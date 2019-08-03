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

            using (var dataContext = directory.GetDataContext())
            {
                // check if the file already exists
                var existingFile = await dataContext.FileRepository.FirstOrDefaultBySpecs(new FindByFilenameSpec(message.Filename),
                    new IncludeFileLocationsSpec());
                if (existingFile == null)
                {
                    return ReturnError(new InvalidOperationError("The file is not indexed.",
                        ErrorCode.FileNotIndexed));
                }

                var fileLocation = existingFile.Files.First(x => directory.PathComparer.Equals(
                    message.Filename, message.Filename));

                existingFile.RemoveLocation(fileLocation.Filename);
                await dataContext.FileRepository.RemoveFileLocation(fileLocation);

                if (!existingFile.Files.Any())
                {
                    await dataContext.FileRepository.Delete(existingFile);
                }
            }

            return new RemoveFileFromIndexResponse();
        }
    }
}
