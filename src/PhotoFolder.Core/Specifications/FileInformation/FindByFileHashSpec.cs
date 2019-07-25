namespace PhotoFolder.Core.Specifications.FileInformation
{
    public class FindByFileHashSpec : BaseSpecification<Domain.Entities.IndexedFile>
    {
        public FindByFileHashSpec(Hash fileHash) : base(x => x.Hash == fileHash.ToString())
        {
        }
    }
}
