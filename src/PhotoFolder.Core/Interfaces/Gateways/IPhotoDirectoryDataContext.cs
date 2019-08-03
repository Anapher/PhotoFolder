using PhotoFolder.Core.Interfaces.Gateways.Repositories;
using System;

namespace PhotoFolder.Core.Interfaces.Gateways
{
    public interface IPhotoDirectoryDataContext : IDisposable
    {
        IIndexedFileRepository FileRepository { get; }
        IFileOperationRepository OperationRepository { get; }
    }
}
