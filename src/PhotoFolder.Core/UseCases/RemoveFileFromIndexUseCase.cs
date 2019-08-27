using System.IO;
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
            if (Path.IsPathFullyQualified(message.RelativeFilename))
                return ReturnError(new InvalidOperationError("The path is fully qualified. Only relative paths are stored", ErrorCode.PathFullyQualified));

            var directory = message.PhotoDirectory;
            using (var dataContext = directory.GetDataContext())
            {
                // check if the file already exists
                var existingFile = await dataContext.FileRepository.FirstOrDefaultBySpecs(new FindByFilenameSpec(message.RelativeFilename),
                    new IncludeFileLocationsSpec());
                if (existingFile == null)
                {
                    return ReturnError(new InvalidOperationError("The file is not indexed.",
                        ErrorCode.FileNotIndexed));
                }

                var fileLocation = existingFile.Files.First(x => directory.PathComparer.Equals(
                    message.RelativeFilename, message.RelativeFilename));

                existingFile.RemoveLocation(fileLocation.RelativeFilename);
                await dataContext.FileRepository.RemoveFileLocation(fileLocation);

                if (!existingFile.Files.Any())
                {
                    await dataContext.FileRepository.Delete(existingFile);
                }

                return new RemoveFileFromIndexResponse(!existingFile.Files.Any());
            }
        }
    }
}
