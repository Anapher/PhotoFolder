namespace PhotoFolder.Core.Interfaces.Services
{
    public interface IPathUtils
    {
        char DirectorySeparator { get; }

        string GetDirectoryName(string path);
        string GetFileName(string path);
        string Combine(params string[] parts);
    }
}