using System.Collections.Generic;
using System.Threading.Tasks;
using PhotoFolder.Core.Shared;

namespace PhotoFolder.Core.Interfaces.Gateways.Repositories
{
    public interface IRepository<T> where T : class, IEntity
    {
        Task<IReadOnlyList<T>> GetAll();
        Task<T?> FirstOrDefaultBySpecs(params ISpecification<T>[] specs);
        Task<IReadOnlyList<T>> GetAllBySpecs(params ISpecification<T>[] specs);
        Task<IReadOnlyList<T>> GetAllReadOnlyBySpecs(params ISpecification<T>[] specs);

        Task<T> Add(T entity);
        Task Update(T entity);
        Task Delete(T entity);
    }
}