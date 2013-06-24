namespace QStore
{
    using System.Collections.Generic;

    public interface IStringSet : ISequenceSet<char>
    {        
        new IEnumerable<string> GetByPrefix(IEnumerable<char> prefix);
    }
}