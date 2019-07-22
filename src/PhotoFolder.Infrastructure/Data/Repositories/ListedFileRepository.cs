using System.Collections.Generic;
using System.Threading.Tasks;
using PhotoFolder.Core.Domain.Entities;
using PhotoFolder.Core.Dto.GatewayResponses.Repositories;
using PhotoFolder.Infrastructure.Shared;

namespace PhotoFolder.Infrastructure.Data.Repositories
{
    public class ListedFileRepository : EfRepository<IndexedFile>, IIndexedFileRepository
    {
        protected ListedFileRepository(AppDbContext appDbContext) : base(appDbContext)
        {
        }

        public Task BatchAdd(IEnumerable<IndexedFile> files)
        {
            throw new System.NotImplementedException();
        }

        public Task BatchDelete(IEnumerable<string> filenames)
        {
            throw new System.NotImplementedException();
        }
    }
}
