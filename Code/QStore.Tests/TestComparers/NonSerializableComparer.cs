namespace QStore.Tests.TestComparers
{
    using System.Collections.Generic;

    public class NonSerializableComparer<T> : IComparer<T>
    {
        public int Compare(T x, T y)
        {
            return Comparer<T>.Default.Compare(x, y);
        }
    }
}