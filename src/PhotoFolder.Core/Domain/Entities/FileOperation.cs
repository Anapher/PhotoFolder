using PhotoFolder.Core.Shared;

namespace PhotoFolder.Core.Domain.Entities
{
    public class FileOperation : BaseEntity
    {
        private FileOperation(IFileReference targetFile, IFileReference? sourceFile, FileOperationType type)
        {
            TargetFilename = targetFile.Filename;
            TargetHash = targetFile.Hash;

            SourceFilename = sourceFile?.Filename;
            SourceHash = sourceFile?.Hash;
            Type = type;
        }

#pragma warning disable CS8618 // Non-nullable field is uninitialized.
        private FileOperation()
        {
        }
#pragma warning restore CS8618 // Non-nullable field is uninitialized.

        public string TargetFilename { get; private set; }
        public string TargetHash { get; private set; }

        public string? SourceFilename { get; private set; }
        public string? SourceHash { get; private set; }

        public FileReference TargetFile
        {
            get => new FileReference(TargetHash, TargetFilename);
        }

        public FileReference? SourceFile { get => SourceFilename == null ? null : new FileReference(SourceHash!, SourceFilename); }

        public FileOperationType Type { get; private set; }

        public static FileOperation FileChanged(IFileReference target, IFileReference source) =>
            new FileOperation(target, source, FileOperationType.Changed);

        public static FileOperation FileMoved(IFileReference target, IFileReference source) =>
            new FileOperation(target, source, FileOperationType.Moved);

        public static FileOperation FileRemoved(IFileReference file) =>
            new FileOperation(file, null, FileOperationType.Removed);

        public static FileOperation NewFile(IFileReference file) =>
            new FileOperation(file, null, FileOperationType.New);
    }

    public enum FileOperationType
    {
        Changed,
        Moved,
        Removed,
        New
    }
}
