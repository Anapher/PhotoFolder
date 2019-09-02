using System.Collections.Generic;
using PhotoFolder.Core.Domain.Entities;
using System.Threading.Tasks;
using PhotoFolder.Core.Domain.Template;

namespace PhotoFolder.Core.Interfaces.Gateways.Repositories
{
    public interface IIndexedFileRepository : IRepository<IndexedFile>
    {
        Task RemoveFileLocation(FileLocation fileLocation);
        Task<IList<string>> FindMatchingDirectories(TemplateString directoryTemplate);
    }
}
