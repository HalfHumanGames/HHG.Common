using HHG.Common.Runtime;
using System.Collections.Generic;
using UnityEngine;

namespace HHG.Common.Runtime
{
    [ExecuteAlways]
    public class BoneDepthSetter : MonoBehaviour
    {
        private const float multiplier = -.001f;

        [System.Serializable]
        private struct BoneDepth
        {
            public Transform Bone;
            public int Depth;

            public BoneDepth(Transform bone)
            {
                Bone = bone;
                Depth = 0;
            }
        }

        [SerializeField] private List<BoneDepth> boneDepths = new List<BoneDepth>();

        private HashSet<Transform> bones = new HashSet<Transform>();

        private void Start()
        {
            RefreshBones();
        }

#if UNITY_EDITOR

        private void OnValidate()
        {
            RefreshBones();
        }

#endif

        private void RefreshBones()
        {
            bones.Clear();

            for (int i = 0; i < boneDepths.Count; i++)
            {
                if (boneDepths[i].Bone == null)
                {
                    boneDepths.RemoveAt(i--);
                    continue;
                }

                bones.Add(boneDepths[i].Bone);
            }

            AddBonesRecursive(transform);
        }

        private void AddBonesRecursive(Transform current)
        {
            if (!bones.Contains(current))
            {
                boneDepths.Add(new BoneDepth(current));
                bones.Add(current);
            }

            foreach (Transform child in current)
            {
                AddBonesRecursive(child);
            }
        }

        private void LateUpdate()
        {
            Transform root = transform.parent;
            Vector3 rootPosition = root.position;
            Vector3 rootForward = root.forward.normalized;

            for (int i = 0; i < boneDepths.Count; i++)
            {
                BoneDepth boneDepth = boneDepths[i];
                Transform bone = boneDepth.Bone;
                Transform boneParent = bone.parent;
                Vector3 worldPosition = rootPosition + rootForward * (boneDepth.Depth * multiplier);
                Vector3 localPosition = boneParent.InverseTransformPoint(worldPosition);
                bone.SetLocalPositionZ(localPosition.z);
            }
        }
    }

}