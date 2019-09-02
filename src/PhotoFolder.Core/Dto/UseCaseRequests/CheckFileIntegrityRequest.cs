using PhotoFolder.Core.Dto.Services;
using PhotoFolder.Core.Dto.UseCaseResponses;
using PhotoFolder.Core.Interfaces;
using PhotoFolder.Core.Services;

namespace PhotoFolder.Core.Dto.UseCaseRequests
{
    public class CheckFileIntegrityRequest : IUseCaseRequest<CheckFileIntegrityResponse>
    {
        public CheckFileIntegrityRequest(FileInformation fileInformation, IFileBaseContext fileBaseContext)
        {
            FileInformation = fileInformation;
            FileBaseContext = fileBaseContext;
        }

        public FileInformation FileInformation { get; }
        public IFileBaseContext FileBaseContext { get; }
    }
}
