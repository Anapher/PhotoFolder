﻿using System.Collections.Generic;
using System.Threading;

namespace PhotoFolder.Application.Utilities
{
    public static class EnumerableExtensions
    {
        public static IEnumerable<T> WithCancellation<T>(this IEnumerable<T> en, CancellationToken token)
        {
            foreach (var item in en)
            {
                token.ThrowIfCancellationRequested();
                yield return item;
            }
        }
    }
}
