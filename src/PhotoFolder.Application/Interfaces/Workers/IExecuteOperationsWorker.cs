using PhotoFolder.Application.Dto.WorkerRequests;
using PhotoFolder.Application.Dto.WorkerResponses;
using PhotoFolder.Application.Dto.WorkerStates;

namespace PhotoFolder.Application.Interfaces.Workers
{
    public interface IExecuteOperationsWorker : IWorker<ExecuteOperationsState, ExecuteOperationsRequest, ExecuteOperationsResponse>
    {
    }
}
