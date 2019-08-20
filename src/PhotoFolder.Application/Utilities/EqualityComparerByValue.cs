using System;
using System.Collections.Generic;

namespace PhotoFolder.Application.Utilities
{
    public class EqualityComparerByValue<T, TValue> : IEqualityComparer<T>
    {
        private readonly Func<T, TValue> _valueSelector;

        public EqualityComparerByValue(Func<T, TValue> valueSelector)
        {
            _valueSelector = valueSelector;
        }

        public bool Equals(T x, T y)
        {
            var val1 = _valueSelector(x);
            var val2 = _valueSelector(y);

            return Equals(val1, val2);
        }

        public int GetHashCode(T obj)
        {
            var value = _valueSelector(obj);
            return value?.GetHashCode() ?? 0;
        }
    }
}
