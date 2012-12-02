namespace QStore
{
    using System.Collections.Generic;

    public interface IStringSet : ISequenceSet<char>
    {
        new string GetByIndex(int index);

        new IEnumerable<string> GetByPrefix(IEnumerable<char> prefix);
    }
}