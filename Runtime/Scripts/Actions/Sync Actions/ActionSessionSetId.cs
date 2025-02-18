using UnityEngine;

namespace HHG.Common.Runtime
{
    [System.Serializable]
    public class ActionSessionSetId : IAction
    {
        [SerializeField] private Session session;
        [SerializeField] private SetMode mode;
        [SerializeField] private string specifiedId;
        [SerializeField] private bool temp;

        public enum SetMode
        {
            MostRecent,
            SiblingIndex,
            SpecifiedId,
        }

        public void Invoke(MonoBehaviour invoker)
        {
            string fileId = mode switch
            {
                SetMode.MostRecent => session.getMostRecentFileId(),
                SetMode.SiblingIndex => invoker.transform.GetSiblingIndex().ToString(),
                SetMode.SpecifiedId => specifiedId,
                _ => session.defaultFileId
            };

            session.fileId = temp ? session.getTempFileId(fileId) : fileId;
        }
    }
}