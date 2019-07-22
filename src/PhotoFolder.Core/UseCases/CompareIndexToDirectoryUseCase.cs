using PhotoFolder.Core.Dto.GatewayResponses.Repositories;
using PhotoFolder.Core.Dto.Services;
using PhotoFolder.Core.Dto.UseCaseRequests;
using PhotoFolder.Core.Dto.UseCaseResponses;
using PhotoFolder.Core.Interfaces;
using PhotoFolder.Core.Interfaces.Gateways;
using PhotoFolder.Core.Interfaces.UseCases;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PhotoFolder.Core.UseCases
{
    public class CompareIndexToDirectoryUseCase : UseCaseStatus<CompareIndexToDirectoryResponse>, ICompareIndexToDirectoryUseCase
    {
        public async Task<CompareIndexToDirectoryResponse?> Handle(CompareIndexToDirectoryRequest message)
        {
            var directory = message.Directory;
            var repository = directory.GetFileRepository();

            // get all files from the repository
            var listedFiles = await repository.GetAll();
            var listedFilesHashSet = new HashSet<IFileInfo>(listedFiles, directory.FileInfoComparer);

            // get all files from the actual directory
            var files = directory.EnumerateFiles().ToList();

            // enumerate and compare
            var newFiles = new List<IFileInfo>();
            foreach (var file in files)
            {
                if (listedFilesHashSet.Remove(file))
                    continue;

                newFiles.Add(file);
            }

            return new CompareIndexToDirectoryResponse(newFiles, listedFilesHashSet);
        }
    }
}
