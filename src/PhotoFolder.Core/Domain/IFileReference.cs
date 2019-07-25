namespace PhotoFolder.Core.Domain
{
    public interface IFileReference
    {
        string Hash { get; }
        string Filename { get; }
    }
}
