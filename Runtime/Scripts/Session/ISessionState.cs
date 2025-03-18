//using System;

//namespace HHG.Common.Runtime
//{
//    public interface ISessionState
//    {
//        string FileName => string.Empty;
//        string TimestampFormatted => string.Empty;
//        DateTime Timestamp => default;

//        void Reset() { }
//        void OnBeforeSave() { }
//        void OnAfterLoad() { }
//    }

//    public interface ISessionState<T> : ISessionState, ICloneable<T> where T : ISessionState<T>
//    {

//    }
//}