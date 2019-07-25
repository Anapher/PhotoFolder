using PhotoFolder.Core.Dto.UseCaseResponses;
using PhotoFolder.Core.Interfaces;
using PhotoFolder.Core.Interfaces.Gateways;

namespace PhotoFolder.Core.Dto.UseCaseRequests
{
    public class RemoveFileFromIndexRequest : IUseCaseRequest<RemoveFileFromIndexResponse>
    {
        public RemoveFileFromIndexRequest(string filename, IPhotoDirectory photoDirectory)
        {
            Filename = filename;
            PhotoDirectory = photoDirectory;
        }

        public string Filename { get; }
        public IPhotoDirectory PhotoDirectory { get; }
    }
}
