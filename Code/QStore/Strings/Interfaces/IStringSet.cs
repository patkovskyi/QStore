namespace QStore.Strings.Interfaces
{
    using System.Collections.Generic;

    using QStore.Core.Interfaces;

    public interface IStringSet : ISequenceSet<char>, IEnumerable<string>
    {
        new IEnumerable<string> GetByPrefix(IEnumerable<char> prefix);
    }
}