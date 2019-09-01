using PhotoFolder.Core.Shared;
using System.Threading.Tasks;

namespace PhotoFolder.Core.Interfaces.Gateways.Repositories
{
    public interface IIdentityRepository<T> : IRepository<T> where T : BaseEntity
    {
        ValueTask<T?> GetById(int id);
    }
}
