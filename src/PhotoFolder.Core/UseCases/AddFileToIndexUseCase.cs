using PhotoFolder.Core.Dto.UseCaseRequests;
using PhotoFolder.Core.Dto.UseCaseResponses;
using PhotoFolder.Core.Interfaces;
using PhotoFolder.Core.Interfaces.UseCases;
using System;
using System.Threading.Tasks;

namespace PhotoFolder.Core.UseCases
{
    public class AddFileToIndexUseCase : UseCaseStatus<AddFileToIndexResponse>, IAddFileToIndexUseCase
    {
        public async Task<AddFileToIndexResponse?> Handle(AddFileToIndexRequest message)
        {
            throw new NotImplementedException();
        }
    }
}
