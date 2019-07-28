using Microsoft.Extensions.DependencyInjection;
using PhotoFolder.Application.Dto.WorkerRequests;
using PhotoFolder.Application.Dto.WorkerResponses;
using PhotoFolder.Application.Dto.WorkerStates;
using PhotoFolder.Application.Interfaces;
using PhotoFolder.Core.Domain.Entities;
using PhotoFolder.Core.Dto.UseCaseRequests;
using PhotoFolder.Core.Dto.UseCaseResponses;
using PhotoFolder.Core.Interfaces.UseCases;
using PhotoFolder.Core.Specifications.FileInformation;
using PhotoFolder.Core.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PhotoFolder.Application.Workers
{
    public class CheckIndexWorker : IWorker<CheckIndexState, CheckIndexRequest, CheckIndexResponse>
    {
        private readonly IServiceProvider _serviceProvider;

        public CheckIndexWorker(IServiceProvider serviceProvider)
        {
            State = new CheckIndexState();
            _serviceProvider = serviceProvider;
        }

        public CheckIndexState State { get; }

        public async Task<CheckIndexResponse> Execute(CheckIndexRequest request, CancellationToken cancellationToken = default)
        {
            var directory = request.Directory;
            var repository = directory.GetFileRepository();

            State.Status = CheckIndexStatus.Querying;

            // get all files from the repository
            var indexedFiles = await repository.GetAllBySpecs(new IncludeFileLocationsSpec());
            var fileInfos = indexedFiles.SelectMany(x => x.Files.Select(y => x.ToFileInformation(y.Filename))).ToList();

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
                    if (response!
                        .EqualFiles.Any() || response.SimilarFiles.Any() || response.RecommendedDirectories.Any() || response.RecommendedFilename != null
                        || response.IsWrongPlaced)
                    {
                        result.Add((fileInformation, response));
                    }
                }

                State.Progress = (float) i / indexedFiles.Count;
            }

            return new CheckIndexResponse(result);
        }
    }
}
