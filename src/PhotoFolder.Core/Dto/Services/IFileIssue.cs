using System.Collections.Generic;

namespace PhotoFolder.Core.Dto.Services
{
    public interface IFileIssue
    {
        /// <summary>
        ///     The identity of this issue. Issues with the same identity are equal.
        /// </summary>
        string Identity { get; }

        /// <summary>
        ///     The file that this issue is based on.
        /// </summary>
        FileInformation File { get; }

        /// <summary>
        ///     Other files that are relevant for this issue. These do not contain the <see cref="File"/>
        /// </summary>
        IEnumerable<FileInformation> RelevantFiles { get; }
    }
}
