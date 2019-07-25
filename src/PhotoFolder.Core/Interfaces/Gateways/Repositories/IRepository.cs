using System.Collections.Generic;
using System.Threading.Tasks;
using PhotoFolder.Core.Shared;

namespace PhotoFolder.Core.Interfaces.Gateways.Repositories
{
    public interface IRepository<T> where T : class, IEntity
    {
        Task<IList<T>> GetAll();
        Task<T?> FirstOrDefaultBySpecs(params ISpecification<T>[] specs);
        Task<IList<T>> GetAllBySpecs(params ISpecification<T>[] spec);

        Task<T> Add(T entity);
        Task Update(T entity);
        Task Delete(T entity);
    }
}