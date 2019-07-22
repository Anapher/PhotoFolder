using PhotoFolder.Core.Dto.GatewayResponses.Repositories;
using PhotoFolder.Core.Dto.Services;
using System.Collections.Generic;

namespace PhotoFolder.Core.Interfaces.Gateways
{
    public interface IPhotoDirectory
    {
        IEqualityComparer<IFileInfo> FileInfoComparer { get; }
        IEnumerable<IFile> EnumerateFiles();

        IIndexedFileRepository GetFileRepository();
    }
}
