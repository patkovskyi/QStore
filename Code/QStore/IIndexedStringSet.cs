namespace QStore
{
    public interface IIndexedStringSet : IIndexedSequenceSet<char>
    {
        new string GetByIndex(long index);
    }
}