using UnityEngine;

namespace HHG.Common
{
    public static class TransformExtensions
    {
        public static Vector3 SnapPosition(this Transform transform, float nearest = 1, bool local = false)
        {
            if (local)
            {
                transform.localPosition = transform.localPosition.Round(nearest);
            }
            else
            {
                transform.position = transform.position.Round(nearest);
            }
            return transform.position;
        }

        public static void Set(this Transform transform, Transform other)
        {
            transform.position = other.position;
            transform.rotation = other.rotation;
            transform.localScale = other.localScale;
        }

        public static void SetLocal(this Transform transform, Transform other)
        {
            transform.localPosition = other.localPosition;
            transform.localRotation = other.localRotation;
            transform.localScale = other.localScale;
        }

        public static void SetPosition(this Transform transform, float x, float y, float z = 0)
        {
            transform.position = new Vector3(x, y, z);
        }

        public static void SetPosition(this Transform transform, Vector3 position)
        {
            transform.position = position;
        }

        public static void SetPosition(this Transform transform, Transform other)
        {
            transform.position = other.position;
        }

        public static void SetLocalPosition(this Transform transform, float x, float y, float z = 0)
        {
            transform.localPosition = new Vector3(x, y, z);
        }

        public static void SetLocalPosition(this Transform transform, Vector3 position)
        {
            transform.localPosition = position;
        }

        public static void SetLocalPosition(this Transform transform, Transform other)
        {
            transform.localPosition = other.localPosition;
        }

        public static void SetPositionX(this Transform transform, float x)
        {
            transform.position = new Vector3(x, transform.position.y, transform.position.z);
        }

        public static void SetPositionY(this Transform transform, float y)
        {
            transform.position = new Vector3(transform.position.x, y, transform.position.z);
        }

        public static void SetPositionZ(this Transform transform, float z)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, z);
        }

        public static void SetLocalPositionX(this Transform transform, float x)
        {
            transform.localPosition = new Vector3(x, transform.localPosition.y, transform.localPosition.z);
        }

        public static void SetLocalPositionY(this Transform transform, float y)
        {
            transform.localPosition = new Vector3(transform.localPosition.x, y, transform.localPosition.z);
        }

        public static void SetLocalPositionZ(this Transform transform, float z)
        {
            transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, z);
        }

        public static void SetRotation(this Transform transform, float x, float y, float z)
        {
            transform.rotation = new Quaternion { eulerAngles = new Vector3(x, y, z) };
        }

        public static void SetRotation(this Transform transform, Vector3 eulers)
        {
            transform.rotation = new Quaternion { eulerAngles = eulers };
        }

        public static void SetRotation(this Transform transform, Quaternion rotation)
        {
            transform.rotation = rotation;
        }

        public static void SetRotation(this Transform transform, Transform other)
        {
            transform.rotation = other.rotation;
        }

        public static void SetLocalRotation(this Transform transform, float x, float y, float z)
        {
            transform.localRotation = new Quaternion { eulerAngles = new Vector3(x, y, z) };
        }

        public static void SetLocalRotation(this Transform transform, Vector3 eulers)
        {
            transform.localRotation = new Quaternion { eulerAngles = eulers };
        }

        public static void SetLocalRotation(this Transform transform, Quaternion rot)
        {
            transform.localRotation = rot;
        }

        public static void SetLocalRotation(this Transform transform, Transform other)
        {
            transform.localRotation = other.localRotation;
        }

        public static void SetRotationX(this Transform transform, float x)
        {
            transform.rotation = new Quaternion { eulerAngles = new Vector3(x, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z) };
        }

        public static void SetRotationY(this Transform transform, float y)
        {
            transform.rotation = new Quaternion { eulerAngles = new Vector3(transform.rotation.eulerAngles.x, y, transform.rotation.eulerAngles.z) };
        }

        public static void SetRotationZ(this Transform transform, float z)
        {
            transform.rotation = new Quaternion { eulerAngles = new Vector3(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, z) };
        }

        public static void SetLocalRotationX(this Transform transform, float x)
        {
            transform.localRotation = new Quaternion { eulerAngles = new Vector3(x, transform.localRotation.eulerAngles.y, transform.localRotation.eulerAngles.z) };
        }

        public static void SetLocalRotationY(this Transform transform, float y)
        {
            transform.localRotation = new Quaternion { eulerAngles = new Vector3(transform.localRotation.eulerAngles.x, y, transform.localRotation.eulerAngles.z) };
        }

        public static void SetLocalRotationZ(this Transform transform, float z)
        {
            transform.localRotation = new Quaternion { eulerAngles = new Vector3(transform.localRotation.eulerAngles.x, transform.localRotation.eulerAngles.y, z) };
        }

        public static void SetLocalScale(this Transform transform, float scale)
        {
            transform.localScale = Vector3.one * scale;
        }

        public static void SetLocalScale(this Transform transform, float x, float y, float z)
        {
            transform.localScale = new Vector3(x, y, z);
        }

        public static void SetLocalScale(this Transform transform, Vector3 scale)
        {
            transform.localScale = scale;
        }

        public static void SetLocalScale(this Transform tranform, Transform other)
        {
            tranform.localScale = other.localScale;
        }

        public static void SetLocalScaleX(this Transform transform, float x)
        {
            transform.localScale = new Vector3(x, transform.localScale.y, transform.localScale.z);
        }

        public static void SetLocalScaleY(this Transform transform, float y)
        {
            transform.localScale = new Vector3(transform.localScale.x, y, transform.localScale.z);
        }

        public static void SetLocalScaleZ(this Transform transform, float z)
        {
            transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y, z);
        }

        public static void SetGlobalScale(this Transform transform, float scale)
        {
            transform.SetGlobalScale(Vector3.one * scale);
        }

        public static void SetGlobalScale(this Transform transform, float x, float y, float z)
        {
            transform.SetGlobalScale(new Vector3(x, y, z));
        }

        public static void SetGlobalScale(this Transform transform, Vector3 scale)
        {
            transform.localScale = Vector3.one;
            Vector3 denominator = transform.lossyScale;
            if (transform.lossyScale.x == 0)
            {
                denominator = denominator.WithX(1);
            }
            if (transform.lossyScale.y == 0)
            {
                denominator = denominator.WithY(1);
            }
            if (transform.lossyScale.z == 0)
            {
                denominator = denominator.WithZ(1);
            }
            transform.localScale = new Vector3(scale.x / denominator.x, scale.y / denominator.y, scale.z / denominator.z);
        }

        public static void SetGlobalScale(this Transform tranform, Transform other)
        {
            tranform.SetGlobalScale(other.lossyScale);
        }

        public static void SetGlobalScaleX(this Transform transform, float x)
        {
            transform.SetGlobalScale(transform.lossyScale.WithX(x));
        }

        public static void SetGlobalScaleY(this Transform transform, float y)
        {
            transform.SetGlobalScale(transform.lossyScale.WithY(y));
        }

        public static void SetGlobalScaleZ(this Transform transform, float z)
        {
            transform.SetGlobalScale(transform.lossyScale.WithZ(z));
        }
    }
}