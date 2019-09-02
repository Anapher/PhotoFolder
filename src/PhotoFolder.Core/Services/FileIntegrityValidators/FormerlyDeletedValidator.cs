using PhotoFolder.Core.Dto.Services;
using PhotoFolder.Core.Dto.Services.FileIssue;
using PhotoFolder.Core.Interfaces.Gateways;
using PhotoFolder.Core.Interfaces.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PhotoFolder.Core.Services.FileIntegrityValidators
{
    public class FormerlyDeletedValidator : IFileIntegrityValidator
    {
        public ValueTask<IEnumerable<IFileIssue>> CheckForIssues(FileInformation file, IFileBaseContext fileBaseContext, IPhotoDirectoryDataContext dataContext)
        {
            return new ValueTask<IEnumerable<IFileIssue>>(CheckForIssues(file, fileBaseContext.PhotoDirectory));
        }

        private IEnumerable<IFileIssue> CheckForIssues(FileInformation file, IPhotoDirectory photoDirectory)
        {
            if (photoDirectory.MemoryManager.DirectoryMemory.DeletedFiles.TryGetValue(file.Hash.ToString(), out var deletedFile))
            {
                yield return new FormerlyDeletedIssue(file, deletedFile);
            }
        }
    }
}
