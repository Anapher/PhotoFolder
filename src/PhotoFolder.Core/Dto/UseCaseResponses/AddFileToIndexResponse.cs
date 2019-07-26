using PhotoFolder.Core.Domain.Entities;

namespace PhotoFolder.Core.Dto.UseCaseResponses
{
    public class AddFileToIndexResponse
    {
        public AddFileToIndexResponse(IndexedFile indexedFile, FileLocation fileLocation)
        {
            IndexedFile = indexedFile;
            FileLocation = fileLocation;
        }

        public IndexedFile IndexedFile { get; }
        public FileLocation FileLocation { get; }

        public void Deconstruct(out IndexedFile indexedFile, out FileLocation fileLocation)
        {
            indexedFile = IndexedFile;
            fileLocation = FileLocation;
        }
    }
}
