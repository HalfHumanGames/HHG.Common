using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace HHG.Common.Runtime
{
    public class UIBinding : MonoBehaviour
    {
        [SerializeField] private List<LabelBinding> labels = new List<LabelBinding>();
        [SerializeField] private List<ImageBinding> images = new List<ImageBinding>();
        [SerializeField] private List<SpriteRendererBinding> spriteRenderers = new List<SpriteRendererBinding>();
        [SerializeField] private List<SliderBinding> sliders = new List<SliderBinding>();
        [SerializeField] private List<DropdownBinding> dropdowns = new List<DropdownBinding>();
        [SerializeField] private List<ToggleBinding> toggles = new List<ToggleBinding>();
        [SerializeField] private List<InputFieldBinding> inputFields = new List<InputFieldBinding>();

        private bool isSetup;

        public void AddBinding(Object bindable, string name, TMP_Text source)
        {
            LabelBinding binding = new LabelBinding(bindable, name, source);
            SetupBinding(binding);
            labels.Add(binding);
        }

        public void AddBinding(Object bindable, string name, Image source)
        {
            ImageBinding binding = new ImageBinding(bindable, name, source);
            SetupBinding(binding);
            images.Add(binding);
        }

        public void AddBinding(Object bindable, string name, SpriteRenderer source)
        {
            SpriteRendererBinding binding = new SpriteRendererBinding(bindable, name, source);
            SetupBinding(binding);
            spriteRenderers.Add(binding);
        }

        public void AddBinding(Object bindable, string name, Slider source)
        {
            SliderBinding binding = new SliderBinding(bindable, name, source);
            SetupBinding(binding);
            sliders.Add(binding);
        }

        public void AddBinding(Object bindable, string name, TMP_Dropdown source)
        {
            DropdownBinding binding = new DropdownBinding(bindable, name, source);
            SetupBinding(binding);
            dropdowns.Add(binding);
        }

        public void AddBinding(Object bindable, string name, Toggle source)
        {
            ToggleBinding binding = new ToggleBinding(bindable, name, source);
            SetupBinding(binding);
            toggles.Add(binding);
        }

        public void AddBinding(Object bindable, string name, TMP_InputField source)
        {
            InputFieldBinding binding = new InputFieldBinding(bindable, name, source);
            SetupBinding(binding);
            inputFields.Add(binding);
        }

        private void SetupBinding(BindingBase binding)
        {
            if (isSetup)
            {
                binding.Validate(this);
                binding.Setup();
            }
        }

        private void Start()
        {
            Validate();
            labels.Setup();
            images.Setup();
            spriteRenderers.Setup();
            sliders.Setup();
            dropdowns.Setup();
            toggles.Setup();
            inputFields.Setup();
            isSetup = true;
        }

        private void OnDestroy()
        {
            labels.Dispose();
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
            labels.Validate(this);
            images.Validate(this);
            spriteRenderers.Validate(this);
            sliders.Validate(this);
            dropdowns.Validate(this);
            toggles.Validate(this);
            inputFields.Validate(this);
        }
    }

    public abstract class BindingBase : System.IDisposable
    {
        public Object Updatable { get => _updatable; set => _updatable = value; }
        public string Name => name;

        // TODO: Make a fancy dropdown that recursively shows
        // all of the _bindable's serializable property paths
        // GetSetMap will also need to be updated to use paths
        [SerializeField] protected Object _updatable;
        [SerializeField] protected string name;

        protected IUpdated updatable => _updatable as IUpdated;

        private object value;

        public void Setup()
        {
            if (updatable != null && updatable.TryGetValueByPath(name, out value))
            {
                updatable.Updated += OnUpdatedInternal;
                OnSetup();
                OnUpdated();
            }
        }

        public void Dispose()
        {
            if (updatable != null)
            {
                updatable.Updated -= OnUpdatedInternal;
            }
        }

        public virtual void OnSetup() { }
        public virtual void Validate(MonoBehaviour mono) { }

        protected abstract void OnUpdated();

        private void OnUpdatedInternal()
        {
            if (updatable.TryGetValueByPath<object>(name, out object val) && value != val)
            {
                value = val;
                OnUpdated();
            }
        }
    }

    public abstract class BindingBase<TValue, TSource> : BindingBase
    {
        [SerializeField] protected TSource source;

        public BindingBase(Object bindable, string name, TSource source)
        {
            _updatable = bindable;
            this.name = name;
            this.source = source;
        }

        public override void Validate(MonoBehaviour mono)
        {
            if (_updatable == null)
            {
                Debug.LogError($"Bindable cannot be null.\n{GetDebugInfo()}", mono);
            }
            else if (updatable == null)
            {
                Debug.LogError($"Bindable does not implement IBindable.\n{GetDebugInfo()}", mono);
            }
            else if(!updatable.TryGetValueByPath(name, out TValue _))
            {
                Debug.LogError($"Property '{name}' not found in bindable.\n{GetDebugInfo()}", mono);
            }
            else if(source == null)
            {
                Debug.LogError($"Source cannot be null.\n{GetDebugInfo()}", mono);
            }
        }

        private string GetDebugInfo()
        {
            return $"_bindable: {_updatable}\nbindable: {updatable}\nname: {name}\nsource: {source}";
        }
    }

    [System.Serializable]
    public class LabelBinding : BindingBase<object, TMP_Text>
    {
        public LabelBinding(Object bindable, string name, TMP_Text source) : base(bindable, name, source)
        {

        }

        protected override void OnUpdated()
        {
            if (updatable.TryGetValueByPath(name, out object val))
            {
                source.text = val.ToString();
            }
        }
    }

    [System.Serializable]
    public class ImageBinding : BindingBase<Sprite, Image>
    {
        public ImageBinding(Object bindable, string name, Image source) : base(bindable, name, source)
        {

        }

        protected override void OnUpdated()
        {
            if (updatable.TryGetValueByPath(name, out Sprite val))
            {
                source.sprite = val;
            }
        }
    }

    [System.Serializable]
    public class SpriteRendererBinding : BindingBase<Sprite, SpriteRenderer>
    {
        public SpriteRendererBinding(Object bindable, string name, SpriteRenderer source) : base(bindable, name, source)
        {

        }

        protected override void OnUpdated()
        {
            if (updatable.TryGetValueByPath(name, out Sprite val))
            {
                source.sprite = val;
            }
        }
    }

    [System.Serializable]
    public class SliderBinding : BindingBase<float, Slider>
    {
        public SliderBinding(Object bindable, string name, Slider source) : base(bindable, name, source)
        {

        }

        public override void OnSetup()
        {
            source.onValueChanged.AddListener(v => updatable.SetValueByPath(name, v));
        }

        protected override void OnUpdated()
        {
            if (updatable.TryGetValueByPath(name, out float val))
            {
                source.SetValueWithoutNotify(val);
            }
        }
    }

    [System.Serializable]
    public class DropdownBinding : BindingBase<int, TMP_Dropdown>
    {
        public DropdownBinding(Object bindable, string name, TMP_Dropdown source) : base(bindable, name, source)
        {

        }

        public override void OnSetup()
        {
            source.onValueChanged.AddListener(v => updatable.SetValueByPath(name, v));
        }

        protected override void OnUpdated()
        {
            if (updatable.TryGetValueByPath(name, out int val))
            {
                source.SetValueWithoutNotify(val);
            }
        }
    }

    [System.Serializable]
    public class ToggleBinding : BindingBase<bool, Toggle>
    {
        public ToggleBinding(Object bindable, string name, Toggle source) : base(bindable, name, source)
        {

        }

        public override void OnSetup()
        {
            source.onValueChanged.AddListener(v => updatable.SetValueByPath(name, v));
        }

        protected override void OnUpdated()
        {
            if (updatable.TryGetValueByPath(name, out bool val))
            {
                source.SetIsOnWithoutNotify(val);
            }
        }
    }

    [System.Serializable]
    public class InputFieldBinding : BindingBase<string, TMP_InputField>
    {
        public InputFieldBinding(Object bindable, string name, TMP_InputField source) : base(bindable, name, source)
        {

        }

        public override void OnSetup()
        {
            source.onValueChanged.AddListener(v => updatable.SetValueByPath(name, v));
        }

        protected override void OnUpdated()
        {
            if (updatable.TryGetValueByPath(name, out string val))
            {
                source.SetTextWithoutNotify(val);
            }
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

        public static void Validate(this IEnumerable<BindingBase> bindings, MonoBehaviour mono)
        {
            foreach (BindingBase binding in bindings)
            {
                binding.Validate(mono);
            }
        }
    }
}