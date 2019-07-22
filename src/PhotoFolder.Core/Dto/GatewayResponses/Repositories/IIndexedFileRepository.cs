using PhotoFolder.Core.Domain.Entities;
using PhotoFolder.Core.Interfaces.Gateways.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PhotoFolder.Core.Dto.GatewayResponses.Repositories
{
    public interface IIndexedFileRepository : IRepository<IndexedFile>
    {
        Task BatchDelete(IEnumerable<string> filenames);
        Task BatchAdd(IEnumerable<IndexedFile> files);
    }
}
