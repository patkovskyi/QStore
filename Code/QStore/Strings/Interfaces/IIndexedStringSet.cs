namespace QStore.Strings.Interfaces
{
    using QStore.Core.Interfaces;

    public interface IIndexedStringSet : IIndexedSequenceSet<char>
    {
        new string GetByIndex(long index);
    }
}