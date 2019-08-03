using PhotoFolder.Core.Domain.Entities;
using System.Threading.Tasks;

namespace PhotoFolder.Core.Interfaces.Gateways.Repositories
{
    public interface IIndexedFileRepository : IRepository<IndexedFile>
    {
        Task RemoveFileLocation(FileLocation fileLocation);
    }
}
