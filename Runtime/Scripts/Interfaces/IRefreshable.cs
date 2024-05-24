namespace HHG.Common.Runtime
{
    public interface IRefreshable
    {
        void Refresh();
    }

    public interface IRefreshableWeak
    {
        void RefreshWeak(object data);
    }

    public interface IRefreshable<T>
    {
        void Refresh(T data);
    }
}