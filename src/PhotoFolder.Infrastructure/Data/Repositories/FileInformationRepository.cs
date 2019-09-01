using System.Threading.Tasks;
using PhotoFolder.Core.Domain.Entities;
using PhotoFolder.Core.Interfaces.Gateways.Repositories;
using PhotoFolder.Infrastructure.Shared;

namespace PhotoFolder.Infrastructure.Data.Repositories
{
    public class IndexedFileRepository : EfRepository<IndexedFile>, IIndexedFileRepository
    {
        public IndexedFileRepository(AppDbContext appDbContext) : base(appDbContext)
        {
        }

        public Task RemoveFileLocation(FileLocation fileLocation)
        {
            _appDbContext.Remove(fileLocation);
            return _appDbContext.SaveChangesAsync();
        }
    }
}
