namespace QStore.Structs
{
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct AlignedPair<TKey, TValue>
    {
        public readonly TKey Key;

        public readonly TValue Value;

        public AlignedPair(TKey key, TValue value)
        {
            this.Key = key;
            this.Value = value;
        }

        public override string ToString()
        {
            return string.Format("<{0},{1}>", this.Key.ToString(), this.Value.ToString());
        }
    }
}