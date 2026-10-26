using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace HHG.Common.Runtime
{
    public class ActionSignalReceiver : MonoBehaviour, INotificationReceiver
    {
        [Serializable]
        public class SignalReaction
        {
            public SignalAsset Signal => signal;
            public ActionEvent Action => action;

            [SerializeField] private SignalAsset signal;
            [SerializeField] private ActionEvent action = new ActionEvent();
        }

        [SerializeField, ReorderableList]
        private List<SignalReaction> reactions = new List<SignalReaction>();

        public void OnNotify(UnityEngine.Playables.Playable origin, INotification notification, object context)
        {
            if (notification is SignalEmitter signalEmitter)
            {
                foreach (SignalReaction reaction in reactions)
                {
                    if (reaction.Signal == signalEmitter.asset)
                    {
                        reaction.Action?.Invoke(this);
                    }
                }
            }
        }
    }
}
