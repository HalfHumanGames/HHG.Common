using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HHG.Common.Runtime
{
    public class AnimationTransformOffsetter : MonoBehaviour
    {
        [System.Serializable]
        public struct Offset
        {
            public Transform Target;
            public Vector3 Position;
            public Vector3 Rotation;
            public Vector3 Scale;

            [System.NonSerialized] public bool HasValue;
            [System.NonSerialized] public Vector3 OriginalPosition;
            [System.NonSerialized] public Vector3 OriginalRotation;
            [System.NonSerialized] public Vector3 OriginalScale;
        }

        [SerializeField, ReorderableList] private List<Offset> offsets = new List<Offset>();

        private void OnEnable()
        {
            StartCoroutine(PostLateUpdate());
        }

        private void LateUpdate()
        {
            for (int i = 0; i < offsets.Count; i++)
            {
                Offset offset = offsets[i];

                if (offset.Target == null || offset.HasValue) continue;

                offset.HasValue = true;
                offset.OriginalPosition = offset.Target.localPosition;
                offset.OriginalRotation = offset.Target.localEulerAngles;
                offset.OriginalScale = offset.Target.localScale;
                offset.Target.localPosition += offset.Position;
                offset.Target.localEulerAngles += offset.Rotation;
                offset.Target.localScale += offset.Scale;
                offsets[i] = offset;
            }
        }
        private IEnumerator PostLateUpdate()
        {
            while (true)
            {
                yield return new WaitForEndOfFrame();

                for (int i = 0; i < offsets.Count; i++)
                {
                    Offset offset = offsets[i];

                    if (offset.Target == null || !offset.HasValue) continue;

                    offset.HasValue = false;
                    offset.Target.localPosition = offset.OriginalPosition;
                    offset.Target.localEulerAngles = offset.OriginalRotation;
                    offset.Target.localScale = offset.OriginalScale;
                    offsets[i] = offset;
                }
            }
        }
    }
}
