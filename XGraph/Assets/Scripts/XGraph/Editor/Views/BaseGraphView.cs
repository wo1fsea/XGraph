using System;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using XGraph.Editor;

namespace XGraph
{
    public class BaseGraphView : GraphView
    {
        public BaseGraphData GraphData { get; private set; }
        public Label GraphNameLabel { get; private set; }
        public string GraphDataFilePath { get; private set;}

        private Dictionary<string, BaseNodeView> nodeViews;

        private MiniMap MiniMap { get; set; }
        
        public virtual Type GraphDataType => typeof(BaseGraphData);

        public BaseGraphData CreateGraphData()
        {
            return Activator.CreateInstance(GraphDataType) as BaseGraphData;
        } 
        
        public BaseGraphView()
        {
            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());
            this.AddManipulator(new ContextualMenuManipulator(OnContextMenuPopulate));

            SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);
            
            graphViewChanged = OnGraphViewChanged;

            nodeViews = new Dictionary<string, BaseNodeView>();
            
            var labelContainer = new VisualElement
            {
                style =
                {
                    position = Position.Absolute,
                    top = 10,
                    left = 10,
                    right = 10
                }
            };
            Insert(0, labelContainer);

            var grid = new GridBackground
            {
                visible = true,
            };
            Insert(0, grid);
            StyleProvider.SetupStyle(grid, "grid-background"); 
            
            var graphNameLabel = new Label
            {
                style =
                {
                    unityTextAlign = TextAnchor.UpperLeft
                }
            };
            labelContainer.Add(graphNameLabel);

            MiniMap = new MiniMap
            {
                maxWidth = 150,
                maxHeight = 75,
                anchored = true,
            };
            UpdateMiniMapPosition();
            RegisterCallback<GeometryChangedEvent>(OnGeometryChanged);
            Add(MiniMap);
            
            GraphNameLabel = graphNameLabel;
        }

        private void OnGeometryChanged(GeometryChangedEvent evt)
        {
            UpdateMiniMapPosition();
        }

        private void UpdateMiniMapPosition()
        {
            var parentWidth = layout.width;
            var parentHeight = layout.height;
            var miniMapWidth = MiniMap.maxWidth;
            var miniMapHeight = MiniMap.maxHeight;
            var miniMapX = parentWidth - miniMapWidth - 10;  // 10 is the margin from the right
            var miniMapY = parentHeight - miniMapHeight - 10;  // 10 is the margin from the bottom

            MiniMap.SetPosition(new Rect(miniMapX, miniMapY, miniMapWidth, miniMapHeight));
        }

        private GraphViewChange OnGraphViewChanged(GraphViewChange graphViewChange)
        {
            if (graphViewChange.elementsToRemove != null)
            {
                foreach (var element in graphViewChange.elementsToRemove)
                {
                    if (element is BaseNodeView view)
                    {
                        GraphData?.nodes.Remove(view.NodeData);
                    }
                    
                    if (element is BaseEdgeView edgeView)
                    {
                        GraphData?.edges.Remove(edgeView.EdgeData);
                    }

                    if (element is StickyNoteView noteView)
                    {
                        GraphData?.stickyNotes.Remove(noteView.StickyNoteData);
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

                        GraphData?.edges.Add(baseEdgeData);
                        var baseEdgeView = CreateEdge(baseEdgeData);
                        edgesToCreate.Add(baseEdgeView);
                    }
                }

                graphViewChange.edgesToCreate = edgesToCreate;
            }

            return graphViewChange;
        }

        private void AddNode(BaseNodeData nodeData)
        {
            GraphData.nodes.Add(nodeData);
            var node = CreateNode(nodeData);
            nodeViews[node.NodeData.guid] = node;
            AddElement(node);
        }
        
        private void AddStickyNote(StickyNoteData stickyNoteData)
        {
            GraphData.stickyNotes.Add(stickyNoteData);
            var stickyNote = CreateStickyNote(stickyNoteData);
            AddElement(stickyNote);
        }

        private void AddEdge(BaseEdgeData baseEdgeData)
        {
            GraphData.edges.Add(baseEdgeData);
            var edge = CreateEdge(baseEdgeData);
            AddElement(edge);
        }

        private void RefreshGraphView(BaseGraphData graphData)
        {
            DeleteElements(graphElements.ToList());
            nodeViews.Clear();

            GraphData = graphData;
            GraphNameLabel.text = GraphData.graphName;
            
            foreach (var nodeData in GraphData.nodes)
            {
                var node = CreateNode(nodeData);
                AddElement(node);
                nodeViews[node.NodeData.guid] = node;
            }

            foreach (var edgeData in GraphData.edges)
            {
                var edge = CreateEdge(edgeData);
                AddElement(edge);
            }

            foreach (var stickyNodeData in GraphData.stickyNotes)
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

            var nodeTypes = NodeProvider.GetCompatibleNodes(GraphData);

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
            File.WriteAllText(filepath, GraphData.ToJson());
            var graphData = GraphData;
            GraphData = null;
            RefreshGraphView(graphData);
        }

        public void LoadFromFile(string filepath)
        {
            string jsonData = File.ReadAllText(filepath);
            RefreshGraphView(BaseGraphData.CreateFromJson(jsonData));
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
        
                
        public void OnNew()
        {
            RefreshGraphView(CreateGraphData());
        }
        
        public void OnSave()
        {
            if (string.IsNullOrEmpty(GraphDataFilePath))
            {
                OnSaveAs();
            }
            else
            {
                SaveToFile(GraphDataFilePath);
            }
        }

        public void OnSaveAs()
        {
            var directory = "Assets";
            if (!string.IsNullOrEmpty(GraphDataFilePath))
            {
                directory = Path.GetDirectoryName(GraphDataFilePath);
            }

            var path = EditorUtility.SaveFilePanel("Save Graph", directory, GraphData.graphName, "json");
            if (!string.IsNullOrEmpty(path))
            {
                GraphDataFilePath = path;
                GraphData.graphName = Path.GetFileNameWithoutExtension(path);
                SaveToFile(path);
            }
        }
        
        public void OnLoad()
        {
            var directory = "Assets";
            if (!string.IsNullOrEmpty(GraphDataFilePath))
            {
                directory = Path.GetDirectoryName(GraphDataFilePath);
            }

            var path = EditorUtility.OpenFilePanel("Select Graph file", directory, "json");
            if (!string.IsNullOrEmpty(path))
            {
                LoadFromFile(path);
                GraphDataFilePath = path;
                GraphData.graphName = Path.GetFileNameWithoutExtension(path);
            }
        }
    }
}