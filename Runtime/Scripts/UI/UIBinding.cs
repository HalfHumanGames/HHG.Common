using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace HHG.Common.Runtime
{
    public abstract class BindingBase : System.IDisposable
    {
        // TODO: Make a fancy dropdown that recursively shows
        // all of the _bindable's serializable property paths
        // GetSetMap will also need to be updated to use paths
        [SerializeField] protected Object _bindable;
        [SerializeField] protected string name;

        protected IBindable bindable => _bindable is IBindableProvider provider ? provider.Bindable : _bindable as IBindable;

        protected abstract void OnUpdated();

        public void Setup()
        {
            if (bindable != null)
            {
                bindable.stateUpdated += OnUpdated;
                OnSetup();
                OnUpdated();
            }
        }

        public void Dispose()
        {
            if (bindable != null)
            {
                bindable.stateUpdated -= OnUpdated;
            }
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
                Debug.LogError($"Bindable cannot be null.\n{GetDebugInfo()}", mono);
                return false;
            }

            if (bindable == null)
            {
                Debug.LogError($"Bindable does not implement IBindable.\n{GetDebugInfo()}", mono);
                return false;
            }

            if (!bindable.TryGetValue(name, out TValue _))
            {
                Debug.LogError($"Property '{name}' not found in bindable.\n{GetDebugInfo()}", mono);
                return false;
            }

            if (source == null)
            {
                Debug.LogError($"Source cannot be null.\n{GetDebugInfo()}", mono);
                return false;
            }

            return true;
        }

        private string GetDebugInfo()
        {
            return $"_bindable: {_bindable}\nbindable: {bindable}\nname: {name}\nsource: {source}";
        }
    }

    [System.Serializable]
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

    [System.Serializable]
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

    [System.Serializable]
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

    [System.Serializable]
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

    [System.Serializable]
    public class DropdownBinding : BindingBase<int, TMP_Dropdown>
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

    [System.Serializable]
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

    [System.Serializable]
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
            texts.Dispose();
            images.Dispose();
            spriteRenderers.Dispose();
            sliders.Dispose();
            dropdowns.Dispose();
            toggles.Dispose();
            inputFields.Dispose();
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