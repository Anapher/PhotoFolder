using PhotoFolder.Core.Interfaces.Gateways;

namespace PhotoFolder.Application.Dto.WorkerRequests
{
    public class CheckIndexRequest
    {
        public CheckIndexRequest(IPhotoDirectory directory)
        {
            Directory = directory;
        }

        public IPhotoDirectory Directory { get; }
    }
}
