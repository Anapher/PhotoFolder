using PhotoFolder.Core.Dto.Services;
using PhotoFolder.Core.Interfaces.Gateways;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PhotoFolder.Infrastructure.State
{
    public class DirectoryScanner
    {
        public async Task<DirectoryScanResult> Scan(IPhotoDirectory photoDirectory)
        {
            // get all files from the repository
            var repository = photoDirectory.GetFileRepository();
            var listedFiles = await repository.GetAll();

            var listedFilesHashSet = new HashSet<IFileInfo>(listedFiles, photoDirectory.FileInfoComparer);

            // get all files from the actual directory
            var files = photoDirectory.EnumerateFiles().ToList();

            // enumerate and compare
            var newFiles = new List<IFileInfo>();
            foreach (var file in files)
            {
                if (listedFilesHashSet.Remove(file))
                    continue;

                newFiles.Add(file);
            }

            return new DirectoryScanResult(newFiles, listedFilesHashSet);
        }
    }

    public class DirectoryScanResult
    {
        public DirectoryScanResult(IEnumerable<IFileInfo> newFiles, IEnumerable<IFileInfo> removedFiles)
        {
            NewFiles = newFiles;
            RemovedFiles = removedFiles;
        }

        public IEnumerable<IFileInfo> NewFiles { get; }
        public IEnumerable<IFileInfo> RemovedFiles { get; }
    }
}
