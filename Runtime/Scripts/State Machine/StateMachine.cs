using System.Collections.Generic;
using UnityEngine;

namespace HHG.Common.Runtime
{
    public class StateMachine<TGameStateId> : MonoBehaviour
    {
        public TGameStateId Initial => initial;
        public TGameStateId Current => current;
        public TGameStateId Previous => previous;

        [SerializeField] protected TGameStateId initial;
        [SerializeField] protected TGameStateId current;
        [SerializeField] protected TGameStateId previous;

        protected GameObject currentGameObject;

        protected virtual void Awake()
        {
            foreach(Transform child in transform)
            {
                child.gameObject.SetActive(false);
            }
        }

        protected virtual void Start()
        {
            GoTo(initial, true);
        }

        public virtual string GetName(TGameStateId stateId)
        {
            return stateId.ToString();
        }

        public GameObject GetState(TGameStateId stateId)
        {
            string name = GetName(stateId);

            if (string.IsNullOrEmpty(name))
            {
                return null;
            }

            Transform state = transform.Find(name);

            return state == null ? null : state.gameObject;
        }

        public void GoTo(TGameStateId stateId, bool force = false)
        {
            if (force || !EqualityComparer<TGameStateId>.Default.Equals(current, stateId))
            {
                previous = current;
                current = stateId;

                GameObject state = GetState(current);

                if (state != null)
                {
                    if (currentGameObject != null)
                    {
                        currentGameObject.SetActive(false);
                    }

                    currentGameObject = state;
                    currentGameObject.SetActive(true);

                    OnTransition(current);
                }
                else
                {
                    Debug.LogError($"State '{current}' not found.", gameObject);
                }
            }
        }

        protected virtual void OnTransition(TGameStateId stateId)
        {

        }
    }
}