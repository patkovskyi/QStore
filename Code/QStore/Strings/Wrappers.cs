namespace QStore.Strings
{
    using System.Collections.Generic;
    using System.Linq;

    internal static class Wrappers
    {
        internal static string Wrap(this char[] sequence)
        {
            return new string(sequence);
        }

        internal static KeyValuePair<string, TValue> Wrap<TValue>(this KeyValuePair<char[], TValue> pair)
        {
            return new KeyValuePair<string, TValue>(new string(pair.Key), pair.Value);
        }

        internal static IEnumerable<string> Wrap(this IEnumerable<char[]> enumerable)
        {
            return enumerable.Select(s => new string(s));
        }

        internal static IEnumerable<KeyValuePair<string, int>> Wrap(
            this IEnumerable<KeyValuePair<char[], int>> enumerable)
        {
            return enumerable.Select(p => new KeyValuePair<string, int>(new string(p.Key), p.Value));
        }

        internal static IEnumerable<KeyValuePair<string, TValue>> Wrap<TValue>(
            this IEnumerable<KeyValuePair<char[], TValue>> enumerable)
        {
            return enumerable.Select(p => new KeyValuePair<string, TValue>(new string(p.Key), p.Value));
        }
    }
}