using System;
using System.Linq;
using System.Numerics;
using System.Reflection;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace XGraph
{
    public static class PropertyFieldGenerator
    {
        // static
        public static VisualElement GeneratePropertyField(BaseNodeData nodeData, FieldInfo field)
        {
            var propertyAttribute =
                Attribute.GetCustomAttribute(field, typeof(PropertyAttribute)) as PropertyAttribute;

            if (propertyAttribute == null) return null;

            VisualElement baseField = null;
            var fieldType = field.FieldType;

            // 检查字段类型并创建相应的输入字段
            if (fieldType == typeof(int))
            {
                var integerField = new IntegerField(propertyAttribute.name);
                integerField.SetValueWithoutNotify((int)field.GetValue(nodeData));
                integerField.RegisterValueChangedCallback(evt => { field.SetValue(nodeData, evt.newValue); });
                
                baseField = integerField;
            }
            else if (fieldType == typeof(long))
            {
                var longField = new LongField(propertyAttribute.name);
                longField.SetValueWithoutNotify((long)field.GetValue(nodeData));
                longField.RegisterValueChangedCallback(evt => { field.SetValue(nodeData, evt.newValue); });
                
                baseField = longField;
            }
            else if (fieldType == typeof(float))
            {
                var floatField = new FloatField(propertyAttribute.name);
                floatField.SetValueWithoutNotify((float)field.GetValue(nodeData));
                floatField.RegisterValueChangedCallback(evt => { field.SetValue(nodeData, evt.newValue); });
                
                baseField = floatField;
            }
            else if (fieldType == typeof(double))
            {
                var floatField = new DoubleField(propertyAttribute.name);
                floatField.SetValueWithoutNotify((double)field.GetValue(nodeData));
                floatField.RegisterValueChangedCallback(evt => { field.SetValue(nodeData, evt.newValue); });
                
                baseField = floatField;
            }
            else if (fieldType == typeof(string))
            {
                var textField = new TextField(propertyAttribute.name);
                textField.SetValueWithoutNotify((string)field.GetValue(nodeData));
                textField.RegisterValueChangedCallback(evt => { field.SetValue(nodeData, evt.newValue); });
                
                baseField = textField;
            }
            else if (fieldType == typeof(bool))
            {
                var toggle = new Toggle(propertyAttribute.name);
                toggle.SetValueWithoutNotify((bool)field.GetValue(nodeData));
                toggle.RegisterValueChangedCallback(evt => { field.SetValue(nodeData, evt.newValue); });
                
                baseField = toggle;
            }
            else if (fieldType == typeof(Vector2))
            {
                var vector2Field = new Vector2Field(propertyAttribute.name);
                var vector2 = (Vector2)field.GetValue(nodeData);
                vector2Field.SetValueWithoutNotify(new UnityEngine.Vector2(
                    vector2.X, vector2.Y));
                vector2Field.RegisterValueChangedCallback(evt =>
                {
                    field.SetValue(nodeData, new Vector2(evt.newValue.x, evt.newValue.y));
                });
                
                baseField = vector2Field;
            }
            else if (fieldType == typeof(Vector3))
            {
                var vector3Field = new Vector3Field(propertyAttribute.name);
                var vector3 = (Vector3)field.GetValue(nodeData);
                vector3Field.SetValueWithoutNotify(new UnityEngine.Vector3(
                    vector3.X, vector3.Y, vector3.Z));
                vector3Field.RegisterValueChangedCallback(evt =>
                {
                    field.SetValue(nodeData, new Vector3(evt.newValue.x, evt.newValue.y, evt.newValue.z));
                });
                
                baseField = vector3Field;
            }
            else if (fieldType.IsEnum)
            {
                var enumValues = Enum.GetValues(fieldType).Cast<Enum>().ToArray();

                var descriptions = enumValues.Select(e =>
                {
                    FieldInfo valueField = e.GetType().GetField(e.ToString());
                    EnumDescriptionAttribute attribute = valueField.GetCustomAttribute<EnumDescriptionAttribute>();
                    return attribute != null ? attribute.description : e.ToString();
                }).ToList();
                var enumValue = (Enum)field.GetValue(nodeData);
                var popupField = new PopupField<string>(propertyAttribute.name, descriptions, Array.IndexOf(Enum.GetValues(fieldType), enumValue));
                popupField.RegisterValueChangedCallback(evt =>
                {
                    var index = descriptions.IndexOf(evt.newValue);
                    field.SetValue(nodeData, enumValues.ElementAt(index));
                });
                
                baseField = popupField;
            }

            return baseField;
        }
    }
}