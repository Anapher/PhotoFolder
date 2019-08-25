using PhotoFolder.Core.Dto.UseCaseResponses;
using PhotoFolder.Core.Interfaces;
using PhotoFolder.Core.Interfaces.Gateways;

namespace PhotoFolder.Core.Dto.UseCaseRequests
{
    public class RemoveFileFromIndexRequest : IUseCaseRequest<RemoveFileFromIndexResponse>
    {
        public RemoveFileFromIndexRequest(string relativeFilename, IPhotoDirectory photoDirectory)
        {
            RelativeFilename = relativeFilename;
            PhotoDirectory = photoDirectory;
        }

        public string RelativeFilename { get; }
        public IPhotoDirectory PhotoDirectory { get; }
    }
}
