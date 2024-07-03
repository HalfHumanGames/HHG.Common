namespace HHG.Common.Runtime
{
    public interface ISessionState<T> : ICloneable<T> where T : ISessionState<T>
    {
        void Reset() { }
        void OnBeforeSave() { }
        void OnAfterLoad() { }
    }
}