namespace PhotoFolder.Core.Domain
{
    public class FileReference : IFileReference
    {
        public FileReference(string fileHash, string filename)
        {
            Hash = fileHash;
            Filename = filename;
        }

#pragma warning disable CS8618 // Non-nullable field is uninitialized.
        protected FileReference()
        {
        }
#pragma warning restore CS8618 // Non-nullable field is uninitialized.

        public string Hash { get; protected set; }
        public string Filename { get; protected set; }
    }
}
