using PhotoFolder.Core.Shared;

namespace PhotoFolder.Core.Domain.Entities
{
    public class FileOperation : BaseEntity
    {
        private FileOperation(FileReference targetFile, FileReference? sourceFile, FileOperationType type)
        {
            TargetFile = targetFile;
            SourceFile = sourceFile;
            Type = type;
        }

#pragma warning disable CS8618 // Non-nullable field is uninitialized.
        private FileOperation()
        {
        }
#pragma warning restore CS8618 // Non-nullable field is uninitialized.

        public FileReference TargetFile { get; private set; }
        public FileReference? SourceFile { get; private set; }

        public FileOperationType Type { get; private set; }

        public static FileOperation FileChanged(FileReference target, FileReference source) =>
            new FileOperation(target, source, FileOperationType.Changed);

        public static FileOperation FileMoved(FileReference target, FileReference source) =>
            new FileOperation(target, source, FileOperationType.Moved);

        public static FileOperation FileRemoved(FileReference file) =>
            new FileOperation(file, null, FileOperationType.Removed);

        public static FileOperation NewFile(FileReference file) =>
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
