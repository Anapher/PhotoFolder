using System.Threading;
using System.Threading.Tasks;

namespace PhotoFolder.Application.Interfaces
{
    public interface IStateful<TState>
    {
        TState State { get; }
    }

    public interface IWorker<TState, TRequest, TResponse> : IStateful<TState>
    {
        Task<TResponse> Execute(TRequest request, CancellationToken cancellationToken = default);
    }
}
