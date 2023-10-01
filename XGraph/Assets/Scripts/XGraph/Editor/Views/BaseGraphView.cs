using System;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;
using System.IO;
using XGraph.Editor;

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
                    if (element is BaseNodeView)
                    {
                        _graphData.nodes.Remove((element as BaseNodeView).NodeData);
                    }
                    
                    if (element is BaseEdgeView)
                    {
                        _graphData.edges.Remove((element as BaseEdgeView).EdgeData);
                    }

                    if (element is StickyNoteView)
                    {
                        _graphData.stickyNotes.Remove((element as StickyNoteView).StickyNoteData);
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
        
        private void AddStickyNote(StickyNoteData stickyNoteData)
        {
            _graphData.stickyNotes.Add(stickyNoteData);
            var stickyNote = CreateStickyNote(stickyNoteData);
            AddElement(stickyNote);
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

            foreach (var stickyNodeData in _graphData.stickyNotes)
            {
                var stickNote = CreateStickyNote(stickyNodeData);
                AddElement(stickNote);
            }
        }

        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
        {
            var compatiblePorts = new List<Port>();

            ports.ForEach(port =>
            {
                if (IsPortCompatible(startPort, port))
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

            var nodeTypes = NodeProvider.GetCompatibleNodes(_graphData);

            foreach (var nodeType in nodeTypes)
            {
                evt.menu.AppendAction($"Create/{nodeType.Item1}", (action) =>
                {
                    BaseNodeData nodeData = System.Activator.CreateInstance(nodeType.Item2) as BaseNodeData;
                    nodeData.editorPosition.X = mousePos.x;
                    nodeData.editorPosition.Y = mousePos.y;
                    AddNode(nodeData);
                });
            }
            
            evt.menu.AppendAction("Create/StickyNote",  (action) =>
            {
                StickyNoteData stickyNoteData = new StickyNoteData();
                stickyNoteData.editorPosition.X = mousePos.x;
                stickyNoteData.editorPosition.Y = mousePos.y;
                AddStickyNote(stickyNoteData);
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
            node.SetPosition(new Rect(new Vector2(nodeData.editorPosition.X, nodeData.editorPosition.Y), Vector2.zero));
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

        public StickyNoteView CreateStickyNote(StickyNoteData stickyNoteData)
        {
            StickyNoteView stickyNote = new StickyNoteView(stickyNoteData);
            stickyNote.SetPosition(new Rect(new Vector2(stickyNoteData.editorPosition.X, stickyNoteData.editorPosition.Y), new Vector2(stickyNoteData.width, stickyNoteData.height)));
            return stickyNote;
        }
        
        public bool IsPortCompatible(Port startPort, Port endPort)
        {
            return startPort != endPort && startPort.node != endPort.node && (
                       (startPort.direction == Direction.Output && endPort.direction == Direction.Input &&
                        endPort.portType.IsAssignableFrom(startPort.portType))
                       || (startPort.direction == Direction.Input && endPort.direction == Direction.Output &&
                           startPort.portType.IsAssignableFrom(endPort.portType))
                        || PortTypeConverter.CanConvert(startPort.portType, endPort.portType));
        }
    }
}