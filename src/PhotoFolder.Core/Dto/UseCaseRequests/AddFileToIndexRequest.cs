using PhotoFolder.Core.Dto.Services;
using PhotoFolder.Core.Dto.UseCaseResponses;
using PhotoFolder.Core.Interfaces;
using PhotoFolder.Core.Interfaces.Gateways;

namespace PhotoFolder.Core.Dto.UseCaseRequests
{
    public class AddFileToIndexRequest : IUseCaseRequest<AddFileToIndexResponse>
    {
        public AddFileToIndexRequest(IFileInfo file, IPhotoDirectory directory)
        {
            File = file;
            Directory = directory;
        }

        public IFileInfo File { get; }
        public IPhotoDirectory Directory { get; }
    }
}
