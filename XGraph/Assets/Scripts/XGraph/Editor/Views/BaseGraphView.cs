using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;
using System.IO;

namespace XGraph
{
    public class MyGraphView : GraphView
    {
        public MyGraphView()
        {
            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());
            this.AddManipulator(new ContextualMenuManipulator(OnContextMenuPopulate));

            this.SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);

            // 添加节点网格
            var grid = new GridBackground();
            grid.visible = true;
            Insert(0, grid);
            grid.StretchToParentSize();
        }

        private Node CreateNode(string name, Vector2 position, bool hasOutputPort = false)
        {
            var node = new Node
            {
                title = name
            };

            node.SetPosition(new Rect(position, new Vector2(150, 100)));

            var inputPort = node.InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Single,
                typeof(float));
            inputPort.portName = "Input";
            node.inputContainer.Add(inputPort);

            if (hasOutputPort)
            {
                var outputPort = node.InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Single,
                    typeof(float));
                outputPort.portName = "Output";
                node.outputContainer.Add(outputPort);
            }

            return node;
        }

        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
        {
            var compatiblePorts = new List<Port>();

            ports.ForEach(port =>
            {
                if (startPort != port && startPort.node != port.node &&
                    startPort.direction != port.direction) // 添加这个条件
                {
                    compatiblePorts.Add(port);
                }
            });

            return compatiblePorts;
        }

        // 处理上下文菜单事件
        private void OnContextMenuPopulate(ContextualMenuPopulateEvent evt)
        {
            // 添加创建节点1的菜单项
            evt.menu.AppendAction("创建节点1", (action) =>
            {
                var node = CreateNode("节点1", evt.localMousePosition, true);
                AddElement(node);
            });

            // 添加创建节点2的菜单项
            evt.menu.AppendAction("创建节点2", (action) =>
            {
                var node = CreateNode("节点2", evt.localMousePosition, false);
                AddElement(node);
            });
        }

        public void SaveToFile(string filepath)
        {
            var nodeDataList = new List<NodeData>();
            var edgeDataList = new List<EdgeData>();

            // 保存节点信息
            nodes.ForEach((node) =>
            {
                var position = node.GetPosition();
                nodeDataList.Add(new NodeData
                    { title = node.title, x = position.x, y = position.y, guid = node.viewDataKey });
            });

            // 保存连接信息
            edges.ForEach((edge) =>
            {
                edgeDataList.Add(new EdgeData
                    { outputNodeGuid = edge.output.node.viewDataKey, inputNodeGuid = edge.input.node.viewDataKey });
            });

            // 将数据序列化为 JSON
            var jsonNodes = JsonUtility.ToJson(new SerializationWrapper<NodeData> { items = nodeDataList });
            var jsonEdges = JsonUtility.ToJson(new SerializationWrapper<EdgeData> { items = edgeDataList });

            // 写入到文件
            File.WriteAllText(filepath + "_nodes.json", jsonNodes);
            File.WriteAllText(filepath + "_edges.json", jsonEdges);
        }

        public void LoadFromFile(string filepath)
        {
            // 读取文件并反序列化数据
            string jsonNodes = File.ReadAllText(filepath + "_nodes.json");
            string jsonEdges = File.ReadAllText(filepath + "_edges.json");

            var nodeDataList = JsonUtility.FromJson<SerializationWrapper<NodeData>>(jsonNodes).items;
            var edgeDataList = JsonUtility.FromJson<SerializationWrapper<EdgeData>>(jsonEdges).items;

            // 清除当前的图
            DeleteElements(graphElements.ToList());

            // 创建并添加节点
            var nodeDict = new Dictionary<string, Node>();
            foreach (var nodeData in nodeDataList)
            {
                var node = CreateNode(nodeData.title, new Vector2(nodeData.x, nodeData.y));
                node.viewDataKey = nodeData.guid;
                nodeDict[nodeData.guid] = node;
                AddElement(node);
            }

            // 创建并添加连接
            foreach (var edgeData in edgeDataList)
            {
                var outputNode = nodeDict[edgeData.outputNodeGuid];
                var inputNode = nodeDict[edgeData.inputNodeGuid];

                // 找到合适的端口进行连接（这里简单地使用了第一个端口，根据你的需求可能需要更复杂的逻辑）
                var edge = new Edge
                {
                    output = outputNode.Q<Port>(),
                    input = inputNode.Q<Port>()
                };
                edge.input.Connect(edge);
                edge.output.Connect(edge);
                AddElement(edge);
            }
        }

        private Node CreateNode(string title, Vector2 position)
        {
            var node = new Node { title = title };
            node.SetPosition(new Rect(position, Vector2.zero));

            // 添加输入端口
            var inputPort = Port.Create<Edge>(Orientation.Horizontal, Direction.Input, Port.Capacity.Single,
                typeof(float));
            inputPort.portName = "Input";
            node.inputContainer.Add(inputPort);

            // 添加输出端口
            var outputPort = Port.Create<Edge>(Orientation.Horizontal, Direction.Output, Port.Capacity.Multi,
                typeof(float));
            outputPort.portName = "Output";
            node.outputContainer.Add(outputPort);

            return node;
        }

        [System.Serializable]
        private class SerializationWrapper<T>
        {
            public List<T> items;
        }
    }
}