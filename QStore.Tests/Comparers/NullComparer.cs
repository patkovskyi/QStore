namespace QStore.Tests.Comparers
{
    using System.Collections.Generic;

    public class NullComparer<T> : Comparer<T>
        where T : class
    {
        public override int Compare(T x, T y)
        {
            if (x == null && y != null)
            {
                return -1;
            }

            if (x != null && y == null)
            {
                return 1;
            }

            return 0;
        }
    }
}