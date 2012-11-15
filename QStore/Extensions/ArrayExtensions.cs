namespace QStore.Extensions
{
    using System;

    internal static class ArrayExtensions
    {
        #region Methods

        internal static void Insert<T>(ref T[] array, T item, int index)
        {
            if (index < array.Length)
            {
                Array.Resize(ref array, array.Length + 1);
                Array.Copy(array, index, array, index + 1, array.Length - index - 1);
            }
            else Array.Resize(ref array, index + 1);
            array[index] = item;
        }

        #endregion
    }
}