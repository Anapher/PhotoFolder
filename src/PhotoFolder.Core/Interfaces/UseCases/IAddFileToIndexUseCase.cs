﻿using PhotoFolder.Core.Dto.UseCaseRequests;
using PhotoFolder.Core.Dto.UseCaseResponses;

namespace PhotoFolder.Core.Interfaces.UseCases
{
    public interface IAddFileToIndexUseCase : IUseCaseRequestHandler<AddFileToIndexRequest, AddFileToIndexResponse>
    {
    }
}
