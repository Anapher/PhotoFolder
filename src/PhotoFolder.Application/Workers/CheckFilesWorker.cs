using Microsoft.Extensions.DependencyInjection;
using PhotoFolder.Application.Dto.WorkerRequests;
using PhotoFolder.Application.Dto.WorkerResponses;
using PhotoFolder.Application.Dto.WorkerStates;
using PhotoFolder.Application.Interfaces.Workers;
using PhotoFolder.Core.Domain.Entities;
using PhotoFolder.Core.Dto.UseCaseRequests;
using PhotoFolder.Core.Dto.UseCaseResponses;
using PhotoFolder.Core.Interfaces.UseCases;
using PhotoFolder.Core.Specifications.FileInformation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using PhotoFolder.Core.Dto.Services;

namespace PhotoFolder.Application.Workers
{
    public class CheckFilesWorker : ICheckFilesWorker
    {
        private readonly IServiceProvider _serviceProvider;

        public CheckFilesWorker(IServiceProvider serviceProvider)
        {
            State = new CheckFilesState();
            _serviceProvider = serviceProvider;
        }

        public CheckFilesState State { get; }

        public async Task<FileCheckReport> Execute(CheckFilesRequest request, CancellationToken cancellationToken = default)
        {
            var directory = request.Directory;
            var fileInfos = request.Files;

            // get all files from the repository
            IList<IndexedFile> indexedFiles;
            using (var context = directory.GetDataContext())
            {
                indexedFiles = await context.FileRepository.GetAllBySpecs(new IncludeFileLocationsSpec());
            }

            State.Status = CheckFilesStatus.Querying;

            var result = new List<(FileInformation, CheckFileIntegrityResponse)>();
            for (int i = 0; i < fileInfos.Count; i++)
            {
                var fileInformation = fileInfos[i];

                var useCase = _serviceProvider.GetRequiredService<ICheckFileIntegrityUseCase>();
                var response = await useCase.Handle(new CheckFileIntegrityRequest(fileInformation, indexedFiles, directory));
                if (useCase.HasError)
                {
                    State.Errors.Add(fileInformation, useCase.Error!);
                }
                else
                {
                    if (response!.EqualFiles.Any() || response.SimilarFiles.Any() || response.IsWrongPlaced)
                    {
                        result.Add((fileInformation, response));
                    }
                }

                State.Progress = (float)i / indexedFiles.Count;
            }

            return new FileCheckReport(result);
        }
    }
}
