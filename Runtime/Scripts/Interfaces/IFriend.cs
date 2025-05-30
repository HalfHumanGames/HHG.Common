namespace Dungeonspire
{
    public interface IFriend<T> where T : class
    {
        public Friendship Friendship { get; }

        public bool IsFriendOf(T subject)
        {
            return Friendship.Friend == subject;
        }

        public void Invoke(T subject, System.Action action)
        {
            if (IsFriendOf(subject))
            {
                action?.Invoke();
            }
            else
            {
                throw new System.InvalidOperationException($"Cannot invoke since not a friend.");
            }
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