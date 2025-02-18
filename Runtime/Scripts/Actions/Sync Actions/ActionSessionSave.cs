using UnityEngine;

namespace HHG.Common.Runtime
{
    [System.Serializable]
    public class ActionSessionSave : IAction
    {
        [SerializeField] private Session session;
        [SerializeField] private Files files = Files.All;
        [SerializeField] private bool saveStagedChanges;

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
                if (saveStagedChanges)
                {
                    session.saveStagedChanges();
                }
                else
                {
                    session.save();
                }
            }

            if (files.HasFlag(Files.Temp))
            {
                session.save(session.tempFileId);
            }
        }
    }
}