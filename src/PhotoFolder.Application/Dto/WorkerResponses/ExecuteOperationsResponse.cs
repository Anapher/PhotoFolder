using System;
using System.Collections.Generic;

namespace PhotoFolder.Application.Dto.WorkerResponses
{
    public class ExecuteOperationsResponse
    {
        public ExecuteOperationsResponse(IReadOnlyDictionary<IFileOperation, Exception> exceptions)
        {
            Exceptions = exceptions;
        }

        public IReadOnlyDictionary<IFileOperation, Exception> Exceptions { get; }
    }
}
