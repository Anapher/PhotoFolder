using System.Collections.Generic;
using System.Threading.Tasks;
using PhotoFolder.Core.Dto.Services;
using PhotoFolder.Core.Interfaces.Gateways;
using PhotoFolder.Core.Services;

namespace PhotoFolder.Core.Interfaces.Services
{
    public interface IFileIntegrityValidator
    {
        ValueTask<IEnumerable<IFileIssue>> CheckForIssues(FileInformation file, IFileBaseContext fileBaseContext, IPhotoDirectoryDataContext dataContext);
    }
}
