using System.Collections.Immutable;
using PhotoFolder.Core.Domain;

namespace PhotoFolder.Core.Interfaces.Services
{
    public interface IDirectoryMemory
    {
        /// <summary>
        ///     Hash -> DeletedFileInfo
        /// </summary>
        IImmutableDictionary<string, DeletedFileInfo> DeletedFiles { get; }

        /// <summary>
        ///      Identity of Issue
        /// </summary>
        IImmutableSet<string> IgnoredIssues { get; }

        IDirectoryMemory SetDeletedFiles(IImmutableDictionary<string, DeletedFileInfo> deletedFiles);
        IDirectoryMemory SetIgnoredIssues(IImmutableSet<string> ignoredIssues);
    }
}
