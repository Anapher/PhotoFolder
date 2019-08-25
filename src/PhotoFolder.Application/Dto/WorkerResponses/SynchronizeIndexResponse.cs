using System.Collections.Generic;
using PhotoFolder.Core.Domain.Entities;

namespace PhotoFolder.Application.Dto.WorkerResponses
{
    public class SynchronizeIndexResponse
    {
        public SynchronizeIndexResponse(IReadOnlyList<FileOperation> operations)
        {
            Operations = operations;
        }

        public IReadOnlyList<FileOperation> Operations { get; }
    }
}
