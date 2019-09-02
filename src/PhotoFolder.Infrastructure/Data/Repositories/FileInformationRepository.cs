using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PhotoFolder.Core.Domain.Entities;
using PhotoFolder.Core.Domain.Template;
using PhotoFolder.Core.Interfaces.Gateways.Repositories;
using PhotoFolder.Infrastructure.Shared;

namespace PhotoFolder.Infrastructure.Data.Repositories
{
    public class IndexedFileRepository : EfRepository<IndexedFile>, IIndexedFileRepository
    {
        public IndexedFileRepository(AppDbContext appDbContext) : base(appDbContext)
        {
        }

        public Task RemoveFileLocation(FileLocation fileLocation)
        {
            _appDbContext.Remove(fileLocation);
            return _appDbContext.SaveChangesAsync();
        }

        public async Task<IList<string>> FindMatchingDirectories(TemplateString directoryTemplate)
        {
            const char escapeChar = '\\';

            var pattern = directoryTemplate.ToLikePattern(escapeChar);
            if (!pattern.EndsWith("%")) pattern += "%"; // as we match file names with a directory pattern

            return await _appDbContext.Set<FileLocation>().Where(x => EF.Functions.Like(x.RelativeFilename, pattern, escapeChar.ToString()))
                .Select(x => x.RelativeFilename.Substring(0, x.RelativeFilename.LastIndexOf('/'))).Distinct().ToListAsync();
        }
    }
}
