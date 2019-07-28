using PhotoFolder.Core.Interfaces.Gateways;

namespace PhotoFolder.Application.Dto.WorkerRequests
{
    public class SynchronizeIndexRequest
    {
        public SynchronizeIndexRequest(IPhotoDirectory directory)
        {
            Directory = directory;
        }

        public IPhotoDirectory Directory { get; }
    }
}
