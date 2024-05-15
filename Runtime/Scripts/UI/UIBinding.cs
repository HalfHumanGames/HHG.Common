using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace HHG.Common.Runtime
{
    public abstract class BindingBase : IDisposable
    {
        // TODO: Make a fancy dropdown that recursively shows
        // all of the _bindable's serializable property paths
        // GetSetMap will also need to be updated to use paths
        [SerializeField] protected Object _bindable;
        [SerializeField] protected string name;

        protected IBindable bindable => _bindable as IBindable;

        protected abstract void OnUpdated();

        public void Setup()
        {
            bindable.Updated += OnUpdated;
            OnSetup();
            OnUpdated();
        }

        public void Dispose()
        {
            bindable.Updated -= OnUpdated;
        }

        public virtual void OnSetup() { }
        public virtual bool Validate(MonoBehaviour mono) => true;
    }

    public abstract class BindingBase<TValue, TSource> : BindingBase
    {
        [SerializeField] protected TSource source;

        public override bool Validate(MonoBehaviour mono)
        {
            if (_bindable == null)
            {
                Debug.LogError("Bindable cannot be null.", mono);
                return false;
            }

            if (bindable == null)
            {
                Debug.LogError("Bindable does not implement IBindable.", mono);
                return false;
            }

            if (!bindable.TryGetValue(name, out TValue value))
            {
                Debug.LogError($"Property '{name}' not found in bindable.", mono);
                return false;
            }

            if (source == null)
            {
                Debug.LogError("Source cannot be null.", mono);
                return false;
            }

            return true;
        }
    }

    [Serializable]
    public class TextBinding : BindingBase<object, TextMeshProUGUI>
    {
        protected override void OnUpdated()
        {
            if (bindable.TryGetValue(name, out object val))
            {
                source.text = val.ToString();
            }
        }
    }

    [Serializable]
    public class ImageBinding : BindingBase<Sprite, Image>
    {
        protected override void OnUpdated()
        {
            if (bindable.TryGetValue(name, out Sprite val))
            {
                source.sprite = val;
            }
        }
    }

    [Serializable]
    public class SpriteRendererBinding : BindingBase<Sprite, SpriteRenderer>
    {
        protected override void OnUpdated()
        {
            if (bindable.TryGetValue(name, out Sprite val))
            {
                source.sprite = val;
            }
        }
    }

    [Serializable]
    public class SliderBinding : BindingBase<float, Slider>
    {
        public override void OnSetup()
        {
            source.onValueChanged.AddListener(v => bindable.SetValue(name, v));
        }

        protected override void OnUpdated()
        {
            if (bindable.TryGetValue(name, out float val))
            {
                source.SetValueWithoutNotify(val);
            }
        }
    }

    [Serializable]
    public class DropdownBinding : BindingBase<int, Dropdown>
    {
        public override void OnSetup()
        {
            source.onValueChanged.AddListener(v => bindable.SetValue(name, v));
        }

        protected override void OnUpdated()
        {
            if (bindable.TryGetValue(name, out int val))
            {
                source.SetValueWithoutNotify(val);
            }
        }
    }

    [Serializable]
    public class ToggleBinding : BindingBase<bool, Toggle>
    {
        public override void OnSetup()
        {
            source.onValueChanged.AddListener(v => bindable.SetValue(name, v));
        }

        protected override void OnUpdated()
        {
            if (bindable.TryGetValue(name, out bool val))
            {
                source.SetIsOnWithoutNotify(val);
            }
        }
    }

    [Serializable]
    public class InputFieldBinding : BindingBase<string, TMP_InputField>
    {
        public override void OnSetup()
        {
            source.onValueChanged.AddListener(v => bindable.SetValue(name, v));
        }

        protected override void OnUpdated()
        {
            if (bindable.TryGetValue(name, out string val))
            {
                source.SetTextWithoutNotify(val);
            }
        }
    }

    public class UIBinding : MonoBehaviour
    {
        [SerializeField] private TextBinding[] texts;
        [SerializeField] private ImageBinding[] images;
        [SerializeField] private SpriteRendererBinding[] spriteRenderers;
        [SerializeField] private SliderBinding[] sliders;
        [SerializeField] private DropdownBinding[] dropdowns;
        [SerializeField] private ToggleBinding[] toggles;
        [SerializeField] private InputFieldBinding[] inputFields;

        private void Start()
        {
            Validate();
            texts.Setup();
            images.Setup();
            spriteRenderers.Setup();
            sliders.Setup();
            dropdowns.Setup();
            toggles.Setup();
            inputFields.Setup();
        }

        private void OnDestroy()
        {
            texts.Setup();
            images.Setup();
            spriteRenderers.Setup();
            sliders.Setup();
            dropdowns.Setup();
            toggles.Setup();
            inputFields.Setup();
        }

        // OnValidate clutters the debug console
        // Instead we call Validate in Start
        private void Validate()
        {
            texts.Validate(this);
            images.Validate(this);
            spriteRenderers.Validate(this);
            sliders.Validate(this);
            dropdowns.Validate(this);
            toggles.Validate(this);
            inputFields.Validate(this);
        }
    }

    public static class BindingBaseExtensions
    {
        public static void Setup(this IEnumerable<BindingBase> bindings)
        {
            foreach (BindingBase binding in bindings)
            {
                binding.Setup();
            }
        }

        public static void Dispose(this IEnumerable<BindingBase> bindings)
        {
            foreach (BindingBase binding in bindings)
            {
                binding.Dispose();
            }
        }

        public static bool Validate(this IEnumerable<BindingBase> bindings, MonoBehaviour mono)
        {
            foreach (BindingBase binding in bindings)
            {
                if (!binding.Validate(mono))
                {
                    return false;
                }
            }

            return true;
        }
    }
}