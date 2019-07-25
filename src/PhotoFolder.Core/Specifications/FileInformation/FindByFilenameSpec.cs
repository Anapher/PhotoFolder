using System.Linq;

namespace PhotoFolder.Core.Specifications.FileInformation
{
    public class FindByFilenameSpec : BaseSpecification<Domain.Entities.IndexedFile>
    {
        public FindByFilenameSpec(string filename) : base(x => x.Files.Any(x => x.Filename == filename))
        {
        }
    }
}
