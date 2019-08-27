using PhotoFolder.Core.Domain;
using System.Collections.Immutable;
using System.Threading.Tasks;

namespace PhotoFolder.Core.Interfaces.Gateways
{
    public interface IDeletedFiles
    {
        IImmutableDictionary<string, DeletedFileInfo> Files { get; }
        Task Update(IImmutableDictionary<string, DeletedFileInfo> files);
    }
}
