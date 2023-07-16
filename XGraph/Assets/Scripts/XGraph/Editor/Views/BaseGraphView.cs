using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;
using System.IO;

namespace XGraph
{
    public class BaseGraphView : GraphView
    {
        private BaseGraphData _graphData;
        public BaseGraphData GraphData => _graphData;

        private Dictionary<string, BaseNodeView> nodeViews;

        public BaseGraphView()
        {
            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());
            this.AddManipulator(new ContextualMenuManipulator(OnContextMenuPopulate));

            SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);

            // 添加节点网格
            var grid = new GridBackground();
            grid.visible = true;
            Insert(0, grid);
            grid.StretchToParentSize();

            graphViewChanged = OnGraphViewChanged;

            _graphData = new BaseGraphData("New Graph");
            nodeViews = new Dictionary<string, BaseNodeView>();
        }

        private GraphViewChange OnGraphViewChanged(GraphViewChange graphViewChange)
        {
            if (graphViewChange.elementsToRemove != null)
            {
                foreach (var element in graphViewChange.elementsToRemove)
                {
                    if (element is BaseEdgeView)
                    {
                        _graphData.edges.Remove((element as BaseEdgeView).EdgeData);
                    }
                }
            }

            if (graphViewChange.edgesToCreate != null)
            {
                List<Edge> edgesToCreate = new List<Edge>();
                foreach (var edge in graphViewChange.edgesToCreate)
                {
                    if (edge is BaseEdgeView)
                    {
                        edgesToCreate.Add(edge);
                    }
                    else
                    {
                        BaseEdgeData baseEdgeData = new BaseEdgeData
                        {
                            inputNodeGuid = (edge.input.node as BaseNodeView)?.NodeData.guid,
                            outputNodeGuid = (edge.output.node as BaseNodeView)?.NodeData.guid,
                            inputPortName = edge.input.portName,
                            outputPortName = edge.output.portName
                        };

                        var baseEdgeView = CreateEdge(baseEdgeData);
                        _graphData.edges.Add(baseEdgeData);
                        edgesToCreate.Add(baseEdgeView);
                    }
                }

                graphViewChange.edgesToCreate = edgesToCreate;
            }

            return graphViewChange;
        }

        private void AddNode(BaseNodeData nodeData)
        {
            _graphData.nodes.Add(nodeData);
            var node = CreateNode(nodeData);
            nodeViews[node.NodeData.guid] = node;
            AddElement(node);
        }

        private void AddEdge(BaseEdgeData baseEdgeData)
        {
            _graphData.edges.Add(baseEdgeData);
            var edge = CreateEdge(baseEdgeData);
            AddElement(edge);
        }

        private void RefreshGraphView()
        {
            DeleteElements(graphElements.ToList());

            foreach (var nodeData in _graphData.nodes)
            {
                var node = CreateNode(nodeData);
                AddElement(node);
                nodeViews[node.NodeData.guid] = node;
            }

            foreach (var edgeData in _graphData.edges)
            {
                var edge = CreateEdge(edgeData);
                AddElement(edge);
            }
        }

        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
        {
            var compatiblePorts = new List<Port>();

            ports.ForEach(port =>
            {
                if (startPort != port && startPort.node != port.node && (
                        (startPort.direction == Direction.Output && port.direction == Direction.Input &&
                         port.portType.IsAssignableFrom(startPort.portType))
                        || (startPort.direction == Direction.Input && port.direction == Direction.Output &&
                            startPort.portType.IsAssignableFrom(port.portType))))
                {
                    compatiblePorts.Add(port);
                }
            });

            return compatiblePorts;
        }

        // 处理上下文菜单事件
        private void OnContextMenuPopulate(ContextualMenuPopulateEvent evt)
        {
            var mousePos =
                (evt.currentTarget as VisualElement).ChangeCoordinatesTo(contentViewContainer, evt.localMousePosition);
            // 添加创建节点1的菜单项
            evt.menu.AppendAction("创建节点1", (action) =>
            {
                NodeData1 nodeData = new NodeData1
                {
                    x = mousePos.x,
                    y = mousePos.y
                };
                AddNode(nodeData);
            });

            // 添加创建节点2的菜单项
            evt.menu.AppendAction("创建节点2", (action) =>
            {
                NodeData2 nodeData = new NodeData2
                {
                    x = mousePos.x,
                    y = mousePos.y
                };
                AddNode(nodeData);
            });
        }

        public void SaveToFile(string filepath)
        {
            File.WriteAllText(filepath, _graphData.ToJson());
        }

        public void LoadFromFile(string filepath)
        {
            // Read the file
            string jsonData = File.ReadAllText(filepath);
            _graphData = BaseGraphData.CreateFromJson(jsonData);
            RefreshGraphView();
        }

        private Port FindPortByName(VisualElement container, string portName)
        {
            foreach (var child in container.Children())
            {
                if (child is Port port && port.portName == portName)
                {
                    return port;
                }
            }

            return null;
        }

        public BaseNodeView CreateNode(BaseNodeData nodeData)
        {
            BaseNodeView node = new BaseNodeView(nodeData);
            node.SetPosition(new Rect(new Vector2(nodeData.x, nodeData.y), Vector2.zero));
            return node;
        }

        public Edge CreateEdge(BaseEdgeData baseEdgeData)
        {
            var outputNode = nodeViews[baseEdgeData.outputNodeGuid];
            var inputNode = nodeViews[baseEdgeData.inputNodeGuid];

            // Find the appropriate ports to connect
            var outputPort = FindPortByName(outputNode.outputContainer, baseEdgeData.outputPortName);
            var inputPort = FindPortByName(inputNode.inputContainer, baseEdgeData.inputPortName);

            // Create the edge and connect it
            var edge = new BaseEdgeView(baseEdgeData)
            {
                output = outputPort,
                input = inputPort
            };
            edge.input.Connect(edge);
            edge.output.Connect(edge);

            return edge;
        }
    }
}