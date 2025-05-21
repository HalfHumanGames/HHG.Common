namespace Dungeonspire
{
    public interface IFriend<T> where T : class
    {
        private Friendship friendship { get => default; }

        public bool IsFriendOf(T subject)
        {
            return friendship.Friend == subject;
        }

        public void SetFriendField<TProperty>(T subject, ref TProperty property, TProperty value)
        {
            property = IsFriendOf(subject) ? value : throw new System.InvalidOperationException($"Cannot set field since not a friend.");
        }
    }

    public struct Friendship : System.IDisposable
    {
        public object Friend => friend;

        private object friend;

        public Friendship(object friend)
        {
            this.friend = friend;
        }

        public Friendship Open(object friend)
        {
            this.friend = friend;
            return this;
        }

        public void Dispose()
        {
            friend = null;
        }
    }
}