using System.Collections.Generic;

namespace PhotoFolder.Application.Utilities
{
    public static class CollectionDiff
    {
        public static (IReadOnlyList<T> newEntries, IReadOnlyCollection<T> removedEntries) Create<T>(
            IEnumerable<T> baseEntries, IEnumerable<T> comparedWithEntries, IEqualityComparer<T> comparer)
        {
            var indexedFilesSet = new HashSet<T>(baseEntries, comparer);

            // enumerate and compare
            var newFiles = new List<T>();
            foreach (var file in comparedWithEntries)
            {
                if (indexedFilesSet.Remove(file))
                    continue;

                newFiles.Add(file);
            }

            return (newFiles, indexedFilesSet);
        }
    }
}
