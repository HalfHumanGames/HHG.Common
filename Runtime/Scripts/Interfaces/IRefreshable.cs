namespace HHG.Common.Runtime
{
    public interface IRefreshable<T>
    {
        void Refresh(T model);
    }
}