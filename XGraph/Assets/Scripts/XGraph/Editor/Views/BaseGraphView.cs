using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

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

            SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);

            // 添加节点网格
            var grid = new GridBackground();
            grid.visible = true;
            Insert(0, grid);
            grid.StretchToParentSize();
        }

        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
        {
            var compatiblePorts = new List<Port>();

            ports.ForEach(port =>
            {
                if (startPort != port && startPort.node != port.node &&
                    startPort.direction != port.direction)
                {
                    compatiblePorts.Add(port);
                }
            });

            return compatiblePorts;
        }

        // 处理上下文菜单事件
        private void OnContextMenuPopulate(ContextualMenuPopulateEvent evt)
        {
            var mousePos = (evt.currentTarget as VisualElement).ChangeCoordinatesTo(contentViewContainer, evt.localMousePosition);
            // 添加创建节点1的菜单项
            evt.menu.AppendAction("创建节点1", (action) =>
            {
                NodeData1 nodeData = new NodeData1
                {
                    nodeType = "NodeData1",
                    stringProp = "example", // Some string value
                    x = mousePos.x,
                    y = mousePos.y,
                };
                var node = CreateNode(nodeData);
                AddElement(node);
            });

            // 添加创建节点2的菜单项
            evt.menu.AppendAction("创建节点2", (action) =>
            {
                NodeData2 nodeData = new NodeData2
                {
                    nodeType = "NodeData2",
                    intProp = 123, // Some int value
                    x = mousePos.x,
                    y = mousePos.y,
                };
                var node = CreateNode(nodeData);
                AddElement(node);
            });
        }

        public void SaveToFile(string filepath)
        {
            var nodeDataList = new List<BaseNodeData>();
            var edgeDataList = new List<EdgeData>();

            // Save node information
            nodes.ForEach((node) =>
            {
                var nodeView = node as BaseNodeView;
                if (nodeView == null)
                {
                    return;
                }
                nodeDataList.Add(nodeView.NodeData);
            });
            // Save connection information
            edges.ForEach((edge) =>
            {
                var outputNode = edge.output.node as BaseNodeView;
                var inputNode = edge.input.node as BaseNodeView;
                if (outputNode == null || inputNode == null)
                {
                    return;
                }
                
                edgeDataList.Add(new EdgeData
                {
                    outputNodeGuid = outputNode.NodeData.guid,
                    outputPortName = edge.output.portName,
                    inputNodeGuid = inputNode.NodeData.guid,
                    inputPortName = edge.input.portName
                });
            });

            // Serialize and write to file
            var graphData = new Dictionary<string, object>
            {
                { "nodes", nodeDataList },
                { "edges", edgeDataList }
            };
    
            string jsonData = JsonConvert.SerializeObject(graphData, Formatting.Indented);

            // Write the JSON string to a file
            File.WriteAllText(filepath, jsonData);
        }

        public void LoadFromFile(string filepath)
        {
            // Read the file
            string jsonData = File.ReadAllText(filepath);
            var graphData = JsonConvert.DeserializeObject<Dictionary<string, object>>(jsonData);

            // Extract nodes and edges
            var nodeDataJsonList = JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(graphData["nodes"].ToString());
            var edgeDataList = JsonConvert.DeserializeObject<List<EdgeData>>(graphData["edges"].ToString());

            // Clear the current graph
            DeleteElements(graphElements.ToList());

            // Create and add nodes
            var nodeDict = new Dictionary<string, Node>();
            foreach (var nodeDataJson in nodeDataJsonList)
            {
                BaseNodeData nodeData;
                string nodeDataJsonString = JsonConvert.SerializeObject(nodeDataJson);
                if (nodeDataJson["nodeType"].ToString() == "NodeData1")
                {
                    nodeData = JsonConvert.DeserializeObject<NodeData1>(nodeDataJsonString);
                }
                else
                {
                    nodeData = JsonConvert.DeserializeObject<NodeData2>(nodeDataJsonString);
                }

                var node = CreateNode(nodeData);
                nodeDict[nodeData.guid] = node;
                AddElement(node);
            }

            // Create and add connections
            foreach (var edgeData in edgeDataList)
            {
                var outputNode = nodeDict[edgeData.outputNodeGuid];
                var inputNode = nodeDict[edgeData.inputNodeGuid];

                // Find the appropriate ports to connect
                var outputPort = FindPortByName(outputNode.outputContainer, edgeData.outputPortName);
                var inputPort = FindPortByName(inputNode.inputContainer, edgeData.inputPortName);

                // Create the edge and connect it
                var edge = new Edge
                {
                    output = outputPort,
                    input = inputPort
                };
                edge.input.Connect(edge);
                edge.output.Connect(edge);

                // Add the edge to the GraphView
                AddElement(edge);
            }
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
        public Node CreateNode(BaseNodeData nodeData)
        {
            BaseNodeView node = new BaseNodeView(nodeData);
            node.SetPosition(new Rect(new Vector2(nodeData.x, nodeData.y), Vector2.zero));
            return node;
        }
    }
}