using UnityEngine;

namespace HHG.Common.Runtime
{
    [System.Serializable]
    public class ActionSessionClear : IAction
    {
        [SerializeField] private Session session;
        [SerializeField] private Files files = Files.All;

        [System.Flags]
        private enum Files
        {
            Main = 1 << 0,
            Temp = 1 << 1,
            All = Main | Temp
        }

        public void Invoke(MonoBehaviour invoker)
        {
            if (files.HasFlag(Files.Main))
            {
                session.clear();
            }

            if (files.HasFlag(Files.Temp))
            {
                session.clear(session.tempFileId);
            }
        }
    }
}