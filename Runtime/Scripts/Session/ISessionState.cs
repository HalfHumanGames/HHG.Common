using System;

namespace HHG.Common.Runtime
{
    public interface ISessionState
    {
        string FileName { get; }
        string TimestampFormatted { get; }
        DateTime Timestamp { get; }

        void Reset() { }
        void OnBeforeSave() { }
        void OnAfterLoad() { }
    }

    public interface ISessionState<T> : ISessionState, ICloneable<T> where T : ISessionState<T>
    {

    }
}