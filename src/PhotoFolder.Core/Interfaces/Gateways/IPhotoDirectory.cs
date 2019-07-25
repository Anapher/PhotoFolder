using PhotoFolder.Core.Dto.Services;
using PhotoFolder.Core.Interfaces.Gateways.Repositories;
using System.Collections.Generic;

namespace PhotoFolder.Core.Interfaces.Gateways
{
    public interface IPhotoDirectory
    {
        IEqualityComparer<string> PathComparer { get; }

        IEnumerable<IFile> EnumerateFiles();
        IFile? GetFile(string filename);

        IIndexedFileRepository GetFileRepository();
        IFileOperationRepository GetOperationRepository();
    }
}
