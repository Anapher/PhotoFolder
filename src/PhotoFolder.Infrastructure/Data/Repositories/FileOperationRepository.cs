using PhotoFolder.Core.Domain.Entities;
using PhotoFolder.Core.Interfaces.Gateways.Repositories;
using PhotoFolder.Infrastructure.Shared;

namespace PhotoFolder.Infrastructure.Data.Repositories
{
    public class FileOperationRepository : EfIdentityRepository<FileOperation>, IFileOperationRepository
    {
        public FileOperationRepository(AppDbContext appDbContext) : base(appDbContext)
        {
        }
    }
}
