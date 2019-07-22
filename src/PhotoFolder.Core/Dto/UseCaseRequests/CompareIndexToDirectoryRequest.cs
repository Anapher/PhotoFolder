using PhotoFolder.Core.Dto.UseCaseResponses;
using PhotoFolder.Core.Interfaces;
using PhotoFolder.Core.Interfaces.Gateways;

namespace PhotoFolder.Core.Dto.UseCaseRequests
{
    public class CompareIndexToDirectoryRequest : IUseCaseRequest<CompareIndexToDirectoryResponse>
    {
        public CompareIndexToDirectoryRequest(IPhotoDirectory directory)
        {
            Directory = directory;
        }

        public IPhotoDirectory Directory { get; }
    }
}
