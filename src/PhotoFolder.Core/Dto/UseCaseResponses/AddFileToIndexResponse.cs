using PhotoFolder.Core.Domain.Entities;

namespace PhotoFolder.Core.Dto.UseCaseResponses
{
    public class AddFileToIndexResponse
    {
        public AddFileToIndexResponse(IndexedFile indexedFile, FileLocation fileLocation, bool isNewFile)
        {
            IndexedFile = indexedFile;
            FileLocation = fileLocation;
            IsNewFile = isNewFile;
        }

        public IndexedFile IndexedFile { get; }
        public FileLocation FileLocation { get; }

        /// <summary>
        ///     True if that file is a completely new file (so it's didnt exist before with a different name)
        /// </summary>
        public bool IsNewFile { get; }

        public void Deconstruct(out IndexedFile indexedFile, out FileLocation fileLocation)
        {
            indexedFile = IndexedFile;
            fileLocation = FileLocation;
        }
    }
}
