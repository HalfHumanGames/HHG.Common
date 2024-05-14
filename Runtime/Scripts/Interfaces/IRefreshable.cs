namespace HHG.Common.Runtime
{
    public interface IRefreshable
    {
        void Refresh();
    }

    public interface IRefreshable<T>
    {
        void Refresh(T model);
    }
}