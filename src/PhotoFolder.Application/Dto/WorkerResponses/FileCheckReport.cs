using PhotoFolder.Core.Domain.Entities;
using PhotoFolder.Core.Dto.UseCaseResponses;
using System.Collections.Generic;
using PhotoFolder.Core.Dto.Services;

namespace PhotoFolder.Application.Dto.WorkerResponses
{
    public class FileCheckReport
    {
        public FileCheckReport(IReadOnlyList<(FileInformation, CheckFileIntegrityResponse)> problematicFiles)
        {
            ProblematicFiles = problematicFiles;
        }

        public IReadOnlyList<(FileInformation, CheckFileIntegrityResponse)> ProblematicFiles { get; }
    }
}
