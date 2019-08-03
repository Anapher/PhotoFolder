using PhotoFolder.Core.Interfaces.Gateways;
using PhotoFolder.Core.Interfaces.Gateways.Repositories;
using PhotoFolder.Infrastructure.Data;
using PhotoFolder.Infrastructure.Data.Repositories;

namespace PhotoFolder.Infrastructure.Photos
{
    public class PhotoDirectoryDataContext : IPhotoDirectoryDataContext
    {
        private readonly AppDbContext _context;

        public PhotoDirectoryDataContext(AppDbContext context)
        {
            _context = context;
        }

        public IIndexedFileRepository FileRepository => new IndexedFileRepository(_context);
        public IFileOperationRepository OperationRepository => new FileOperationRepository(_context);

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
