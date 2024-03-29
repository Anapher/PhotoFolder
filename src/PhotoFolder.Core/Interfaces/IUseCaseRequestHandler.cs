using System.Threading.Tasks;

namespace PhotoFolder.Core.Interfaces
{
    public interface IUseCaseRequestHandler<in TUseCaseRequest, TUseCaseResponse> : IUseCaseErrors where TUseCaseRequest : IUseCaseRequest<TUseCaseResponse> where TUseCaseResponse : class
    {
        /// <summary>
        ///     Handle the use case
        /// </summary>
        /// <param name="message">The input data for the use case</param>
        /// <returns>Return the response, or null if an error occurred</returns>
        Task<TUseCaseResponse?> Handle(TUseCaseRequest message);
    }
}
