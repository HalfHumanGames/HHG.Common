namespace HHG.Common.Runtime
{
    public interface IAggregatable
    {
        object Seed() => this;
        object Aggregate(object other);
    }

    public interface IAggregatable<T>
    {
        T Seed() => (T)this;
        T Aggregate(T other);
    }
}