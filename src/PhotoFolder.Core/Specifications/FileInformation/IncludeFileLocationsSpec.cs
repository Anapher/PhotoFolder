namespace PhotoFolder.Core.Specifications.FileInformation
{
    public class IncludeFileLocationsSpec : BaseSpecification<Domain.Entities.IndexedFile>
    {
        public IncludeFileLocationsSpec() : base(x => true)
        {
            AddInclude(x => x.Files);
        }
    }
}
