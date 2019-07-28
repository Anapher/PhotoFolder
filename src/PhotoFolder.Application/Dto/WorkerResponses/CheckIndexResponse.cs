using PhotoFolder.Core.Domain.Entities;
using PhotoFolder.Core.Dto.UseCaseResponses;
using System.Collections.Generic;

namespace PhotoFolder.Application.Dto.WorkerResponses
{
    public class CheckIndexResponse
    {
        public CheckIndexResponse(IReadOnlyList<(FileInformation, CheckFileIntegrityResponse)> problematicFiles)
        {
            ProblematicFiles = problematicFiles;
        }

        public IReadOnlyList<(FileInformation, CheckFileIntegrityResponse)> ProblematicFiles { get; }
    }
}
