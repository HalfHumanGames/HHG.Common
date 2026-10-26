namespace HHG.Common.Runtime
{
    [System.Serializable]
    public struct Weighted<T>
    {
        public T Value;
        public int Weight;

        public Weighted(T value, int weight)
        {
            Value = value;
            Weight = weight;
        }
    }
}
