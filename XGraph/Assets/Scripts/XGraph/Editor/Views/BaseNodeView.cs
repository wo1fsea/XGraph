using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;
using UnityEngine;

namespace XGraph
{
    public class BaseNodeView : Node
    {
        private BaseNodeData _nodeData;
        public event System.Action<Vector2> OnPositionChanged;
        
        public BaseNodeView(BaseNodeData nodeData)
        {
            _nodeData = nodeData;
            
            // 设置节点标题
            title = nodeData.title;

            // 根据不同的数据类型动态添加编辑框
            if (nodeData is NodeData1)
            {
                var stringData = nodeData as NodeData1;
                TextField textField = new TextField("String Property:");
                textField.value = stringData.stringProp;
                textField.RegisterValueChangedCallback(evt => { stringData.stringProp = evt.newValue; });
                this.extensionContainer.Add(textField);
            }
            else if (nodeData is NodeData2)
            {
                var intData = nodeData as NodeData2;
                TextField textField = new TextField("Integer Property:");
                textField.value = intData.intProp.ToString();
                textField.RegisterValueChangedCallback(evt =>
                {
                    int result;
                    if (int.TryParse(evt.newValue, out result))
                    {
                        intData.intProp = result;
                    }
                    else
                    {
                        textField.value = intData.intProp.ToString(); // Reset to valid value if parsing fails
                    }
                });
                this.extensionContainer.Add(textField);
            }
            
            // 添加输入端口
            var inputPort = Port.Create<Edge>(Orientation.Horizontal, Direction.Input, Port.Capacity.Multi, typeof(float));
            inputPort.portName = "Input";
            inputContainer.Add(inputPort);

            // 添加输出端口
            var outputPort = Port.Create<Edge>(Orientation.Horizontal, Direction.Output, Port.Capacity.Multi, typeof(float));
            outputPort.portName = "Output";
            outputContainer.Add(outputPort);
            
            // 监听位置变化事件
            this.RegisterCallback<GeometryChangedEvent>(evt =>
            {
                OnPositionChanged?.Invoke(evt.newRect.position);
            });
            
            // 刷新UI
            this.RefreshExpandedState();
            this.RefreshPorts();
        }
       
        public void UpdateNodeDataPosition(Vector2 newPosition)
        {
            _nodeData.x = newPosition.x;
            _nodeData.y = newPosition.y;
        }
        
        public BaseNodeData NodeData
        {
            get { return _nodeData; }
        }
    }
}