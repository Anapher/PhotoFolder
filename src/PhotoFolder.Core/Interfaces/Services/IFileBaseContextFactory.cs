using System.Collections.Generic;
using System.Threading.Tasks;
using PhotoFolder.Core.Domain.Entities;
using PhotoFolder.Core.Interfaces.Gateways;
using PhotoFolder.Core.Services;

namespace PhotoFolder.Core.Interfaces.Services
{
    public interface IFileBaseContextFactory
    {
        ValueTask<IFileBaseContext> BuildContext(IPhotoDirectory photoDirectory);
        ValueTask<IFileBaseContext> BuildContext(IPhotoDirectory photoDirectory, IReadOnlyList<IndexedFile> indexedFiles);
    }
}
