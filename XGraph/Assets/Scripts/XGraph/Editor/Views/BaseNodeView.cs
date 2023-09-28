using UnityEditor.Experimental.GraphView;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEngine;
using System;
using System.Reflection;
using UnityEditor.UIElements;

namespace XGraph
{
    public class BaseNodeView : Node
    {
        private BaseNodeData _nodeData;
        public BaseNodeData NodeData => _nodeData;
        public event Action<Vector2> OnPositionChanged;

        public BaseNodeView(BaseNodeData nodeData)
        {
            _nodeData = nodeData;

            // 监听位置变化事件
            RegisterCallback<GeometryChangedEvent>(evt =>
            {
                _nodeData.x = evt.newRect.x;
                _nodeData.y = evt.newRect.y;
                OnPositionChanged?.Invoke(evt.newRect.position);
            });

            // 设置节点标题
            title = nodeData.Title;

            HandleNodeDataAttributes(_nodeData);

            // 刷新UI
            RefreshExpandedState();
            RefreshPorts();
        }

        private void HandleNodeDataAttributes(BaseNodeData nodeData)
        {
            FieldInfo[] fields = nodeData.GetType()
                .GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

            foreach (FieldInfo field in fields)
            {
                if (Attribute.GetCustomAttribute(field, typeof(PropertyAttribute)) is PropertyAttribute
                    propertyAttribute)
                { 
                    VisualElement baseField = null;
                    var fieldType = field.FieldType;
                
                    // 检查字段类型并创建相应的输入字段
                    if (fieldType == typeof(int))
                    {
                        var integerField = new IntegerField(propertyAttribute.name);
                        integerField.SetValueWithoutNotify((int) field.GetValue(nodeData));
                        integerField.RegisterValueChangedCallback(evt =>
                        {
                            field.SetValue(nodeData, evt.newValue);
                        });
                        baseField = integerField;
                    }
                    else if (fieldType == typeof(long))
                    {
                        var longField = new LongField(propertyAttribute.name);
                        longField.SetValueWithoutNotify((long) field.GetValue(nodeData));
                        longField.RegisterValueChangedCallback(evt =>
                        {
                            field.SetValue(nodeData, evt.newValue);
                        });
                        baseField = longField;
                    }
                    else if (fieldType == typeof(float))
                    {
                        var floatField = new FloatField(propertyAttribute.name);
                        floatField.SetValueWithoutNotify((float) field.GetValue(nodeData));
                        floatField.RegisterValueChangedCallback(evt =>
                        {
                            field.SetValue(nodeData, evt.newValue);
                        });
                        baseField = floatField;
                    }
                    else if (fieldType == typeof(double))
                    {
                        var floatField = new DoubleField(propertyAttribute.name);
                        floatField.SetValueWithoutNotify((double) field.GetValue(nodeData));
                        floatField.RegisterValueChangedCallback(evt =>
                        {
                            field.SetValue(nodeData, evt.newValue);
                        });
                        baseField = floatField;
                    }
                    else if (fieldType == typeof(string))
                    {
                        var textField = new TextField(propertyAttribute.name);
                        textField.SetValueWithoutNotify((string) field.GetValue(nodeData));
                        textField.RegisterValueChangedCallback(evt =>
                        {
                            field.SetValue(nodeData, evt.newValue);
                        });
                        baseField = textField;
                
                    }
                    else if (fieldType == typeof(bool))
                    {
                        var toggle = new Toggle(propertyAttribute.name);
                        toggle.SetValueWithoutNotify((bool) field.GetValue(nodeData));
                        toggle.RegisterValueChangedCallback(evt =>
                        {
                            field.SetValue(nodeData, evt.newValue);
                        });
                        baseField = toggle;
                        
                    }
                    // ... 其他类型
                
                    if (baseField != null)
                    {
                        var inputElement = baseField.Q(TextField.textInputUssName);
                        inputElement.style.minWidth = 50;
                        contentContainer.Add(baseField);
                    }
                }

                if (Attribute.GetCustomAttribute(field, typeof(InputAttribute)) is InputAttribute inputAttribute)
                {
                    // 添加输出端口
                    Port.Capacity capacity =
                        inputAttribute.multipleConnect ? Port.Capacity.Multi : Port.Capacity.Single;
                    var inputPort = GeneratePort(Orientation.Horizontal, Direction.Input, capacity, field.FieldType);
                    inputPort.portName = inputAttribute.name;
                    inputContainer.Add(inputPort);
                    continue;
                }

                if (Attribute.GetCustomAttribute(field, typeof(OutputAttribute)) is OutputAttribute outputAttribute)
                {
                    // 添加输出端口
                    Port.Capacity capacity =
                        outputAttribute.multipleConnect ? Port.Capacity.Multi : Port.Capacity.Single;
                    var outputPort = GeneratePort(Orientation.Horizontal, Direction.Output, capacity, field.FieldType);
                    outputPort.portName = outputAttribute.name;
                    outputContainer.Add(outputPort);
                    continue;
                }
            }
        }

        private Port GeneratePort(Orientation orientation, Direction direction, Port.Capacity capacity,
            Type type)
        {
            return new BasePortView(orientation, direction, capacity, type);
        }
    }
}