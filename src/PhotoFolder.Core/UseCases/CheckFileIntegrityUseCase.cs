using PhotoFolder.Core.Dto;
using PhotoFolder.Core.Dto.UseCaseRequests;
using PhotoFolder.Core.Dto.UseCaseResponses;
using PhotoFolder.Core.Interfaces;
using PhotoFolder.Core.Interfaces.UseCases;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PhotoFolder.Core.UseCases
{
    public class CheckFileIntegrityUseCase : UseCaseStatus<CheckFileIntegrityResponse>, ICheckFileIntegrityUseCase
    {
        public Task<CheckFileIntegrityResponse> Handle(CheckFileIntegrityRequest message)
        {
            var directory = message.PhotoDirectory;


        }
    }
}
