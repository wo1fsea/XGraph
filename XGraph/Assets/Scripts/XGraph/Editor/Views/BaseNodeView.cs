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
                _nodeData.editorPosition.X = evt.newRect.x;
                _nodeData.editorPosition.Y = evt.newRect.y;
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
            foreach (FieldInfo field in nodeData.GetFields())
            {
                if (Attribute.GetCustomAttribute(field, typeof(PropertyAttribute)) is PropertyAttribute
                    propertyAttribute)
                {
                    var baseField = PropertyFieldGenerator.GeneratePropertyField(nodeData, field);
                
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