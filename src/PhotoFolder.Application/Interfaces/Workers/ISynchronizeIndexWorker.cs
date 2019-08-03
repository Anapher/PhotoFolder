using PhotoFolder.Application.Dto.WorkerRequests;
using PhotoFolder.Application.Dto.WorkerResponses;
using PhotoFolder.Application.Dto.WorkerStates;

namespace PhotoFolder.Application.Interfaces.Workers
{
    public interface ISynchronizeIndexWorker : IWorker<SynchronizeIndexState, SynchronizeIndexRequest, SynchronizeIndexResponse>
    {
    }
}
