using System.Collections.Generic;
using UnityEngine;

namespace HHG.Common
{
    public class StateMachine<TGameStateId> : MonoBehaviour
    {
        public TGameStateId Initial => initial;
        public TGameStateId Current => current;
        public TGameStateId Previous => previous;

        [SerializeField] private TGameStateId initial;
        [SerializeField] private TGameStateId current;
        [SerializeField] private TGameStateId previous;

        private GameObject currentGameObject;

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
            if (!EqualityComparer<TGameStateId>.Default.Equals(current, stateId) || force)
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