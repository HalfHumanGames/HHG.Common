using HHG.Common.Runtime;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace HHG.Common.Editor
{
    [CustomPropertyDrawer(typeof(FilterAttribute))]
    public class FilterDrawer : PropertyDrawer
    {
        private FilterAttribute filter => attribute as FilterAttribute;

        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            VisualElement container = new VisualElement();
            ObjectField field = new ObjectField(property.displayName);
            field.AddToClassList(BaseField<Object>.alignedFieldUssClassName);
            field.objectType = filter.Type;
            field.BindProperty(property);
            container.Add(field);
            return container;
        }
    }
}
