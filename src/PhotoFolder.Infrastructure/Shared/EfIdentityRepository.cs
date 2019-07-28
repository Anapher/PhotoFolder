#pragma warning disable CS8619 // Nullability of reference types in value doesn't match target type.
using PhotoFolder.Core.Interfaces.Gateways.Repositories;
using PhotoFolder.Core.Shared;
using PhotoFolder.Infrastructure.Data;
using System.Threading.Tasks;

namespace PhotoFolder.Infrastructure.Shared
{
    public class EfIdentityRepository<T> : EfRepository<T>, IIdentityRepository<T> where T : BaseEntity
    {
        protected EfIdentityRepository(AppDbContext appDbContext) : base(appDbContext)
        {
        }

        public virtual Task<T?> GetById(int id)
        {
            return _appDbContext.Set<T>().FindAsync(id);
        }
    }
}
