//using UnityEngine;
//using System.Collections.Generic;
//using System;

//namespace HHG.Common.Runtime
//{
//    /// <summary>
//    /// Convenience way to create objects in the hierarchy that are a bit like prefabs, except that they
//    /// aren't. They exist in the scene, immediately deactivate themselves, then provide an easy way to
//    /// instantiate a copy in the same place in the hierarchy.
//    /// If you call ReturnToPool() once you're done with the instance, then it's returned
//    /// to the original prototype to be reused if necessary instead of creating a brand new GameObject.
//    /// </summary>
//    public class Prototype : MonoBehaviour
//    {
//        public bool IsOriginal => original == null;
//        public Prototype Original => original;
//        public event Action OnReturnToPool;

//        private Prototype original;
//        private List<Prototype> pool;

//        void Start()
//        {
//            if (IsOriginal)
//            {
//                gameObject.SetActive(false);
//            }
//        }

//        void OnDestroy()
//        {
//            if (pool != null)
//            {
//                foreach (Prototype proto in pool)
//                {
//                    Destroy(proto);
//                }
//            }
//        }

//        public T Instantiate<T>() where T : Component
//        {
//            Prototype proto;

//            // Re-use instance from pool
//            if (pool != null && pool.Count > 0)
//            {
//                var instanceIdx = pool.Count - 1;
//                proto = pool[instanceIdx];
//                pool.RemoveAt(instanceIdx);
//            }

//            // Instantiate fresh instance
//            else
//            {
//                proto = Instantiate(this);

//                proto.transform.SetParent(transform.parent, worldPositionStays: false);

//                var rectTransform = transform as RectTransform;
//                if (rectTransform)
//                {
//                    var protoRectTransform = proto.transform as RectTransform;
//                    protoRectTransform.anchorMin = rectTransform.anchorMin;
//                    protoRectTransform.anchorMax = rectTransform.anchorMax;
//                    protoRectTransform.pivot = rectTransform.pivot;
//                    protoRectTransform.sizeDelta = rectTransform.sizeDelta;
//                }

//                proto.transform.localPosition = transform.localPosition;
//                proto.transform.localRotation = transform.localRotation;
//                proto.transform.localScale = transform.localScale;

//                proto.original = this;
//            }

//            proto.gameObject.SetActive(true);

//            return proto.GetComponent<T>();
//        }

//        public void ReturnToPool()
//        {
//            if (IsOriginal)
//            {
//                Debug.LogError("Can't return to pool because the original prototype doesn't exist. Is this prototype the original?");
//                Destroy(gameObject);
//                return;
//            }

//            original.AddToPool(this);
//        }

//        public T GetOriginal<T>()
//        {
//            return IsOriginal ? GetComponent<T>() : Original.GetComponent<T>();
//        }

//        void AddToPool(Prototype proto)
//        {
//            if (!IsOriginal)
//            {
//                Debug.LogError("Adding " + proto.name + " to prototype pool of " + name + " but this appears to be an instance itself?");
//            }

//            proto.gameObject.SetActive(false);

//            if (pool == null)
//            {
//                pool = new List<Prototype>();
//            }

//            pool.Add(proto);

//            proto.OnReturnToPool?.Invoke();
//        }
//    } 
//}