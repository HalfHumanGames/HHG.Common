namespace HHG.Common.Runtime
{
    public interface IAggregatable
    {
        object Aggregate(object other);
    }

    public interface IAggregatable<T>
    {
        T Aggregate(T other);
    }
}