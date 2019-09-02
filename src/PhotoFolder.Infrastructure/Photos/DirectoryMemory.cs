using System.Collections.Immutable;
using PhotoFolder.Core.Domain;
using PhotoFolder.Core.Dto.Services;
using PhotoFolder.Core.Interfaces.Services;

namespace PhotoFolder.Infrastructure.Photos
{
   public class DirectoryMemory : IDirectoryMemory
    {
        public DirectoryMemory(IImmutableDictionary<string, DeletedFileInfo> deletedFiles, IImmutableSet<string> ignoredIssues)
        {
            DeletedFiles = deletedFiles;
            IgnoredIssues = ignoredIssues;
        }

        // For serialization
#pragma warning disable CS8618 // Non-nullable field is uninitialized.
        private DirectoryMemory()
        {
        }
#pragma warning restore CS8618 // Non-nullable field is uninitialized.

        public static IDirectoryMemory Empty =>
            new DirectoryMemory(ImmutableDictionary<string, DeletedFileInfo>.Empty, ImmutableHashSet<string>.Empty);

        public IImmutableDictionary<string, DeletedFileInfo> DeletedFiles { get; }
        public IImmutableSet<string> IgnoredIssues { get; }

        public IDirectoryMemory SetDeletedFiles(IImmutableDictionary<string, DeletedFileInfo> deletedFiles) =>
            new DirectoryMemory(deletedFiles, IgnoredIssues);

        public IDirectoryMemory SetIgnoredIssues(IImmutableSet<string> ignoredIssues) => new DirectoryMemory(DeletedFiles, ignoredIssues);
    }
}
