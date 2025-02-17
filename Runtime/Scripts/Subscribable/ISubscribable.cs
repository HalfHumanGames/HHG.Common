namespace HHG.Common.Runtime
{
    public interface ISubscribable
    {
        public void Subscribe(System.Action callback);
        public void Unsubscribe(System.Action callback);
    }
}