using System;
using System.Collections.Generic;
using System.Reflection;
using Newtonsoft.Json;

namespace XGraph
{
    [Serializable]
    public class BaseGraphData
    {
        public string graphName;

        public List<BaseNodeData> nodes = new();
        public List<BaseEdgeData> edges = new();

        public List<StickyNoteData> stickyNotes = new();

        public BaseGraphData(string graphName)
        {
            this.graphName = graphName;
        }

        public static BaseGraphData CreateFromJson(string json)
        {
            return JsonConvert.DeserializeObject<BaseGraphData>(json, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto
            });
        }

        public string ToJson()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto
            });
        }

        public void Run()
        {
            var runtimeData = BaseGraphRuntimeData.ConstructFromGraphData(this);
            runtimeData.ProcessFlow();
        }
    }

    public class BaseGraphRuntimeData
    {
        public Dictionary<String, BaseNodeData> nodes = new();

        public Dictionary<String, List<BaseEdgeData>> outEdges = new();
        public Dictionary<String, BaseEdgeData> inEdges = new();

        public List<FlowNode> flowNodes = new();

        public static BaseGraphRuntimeData ConstructFromGraphData(BaseGraphData graphData)
        {
            BaseGraphRuntimeData runtimeData = new BaseGraphRuntimeData();
            List<FlowNode> curFlowNodes = new();
            foreach (var nodeData in graphData.nodes)
            {
                runtimeData.nodes.Add(nodeData.guid, nodeData);

                if (nodeData is StartFlowNode flowNode)
                {
                    curFlowNodes.Add(flowNode);
                }
            }

            foreach (var edgeData in graphData.edges)
            {
                if (runtimeData.outEdges.TryGetValue(edgeData.GetOutputPortKey(), out var edgeList))
                {
                    edgeList.Add(edgeData);
                }
                else
                {
                    runtimeData.outEdges.Add(edgeData.GetOutputPortKey(), new List<BaseEdgeData> { edgeData });
                }

                runtimeData.inEdges.Add(edgeData.GetInputPortKey(), edgeData);
            }

            while (curFlowNodes.Count != 0)
            {
                List<FlowNode> nextFlowNodes = new();

                foreach (var curFlowNode in curFlowNodes)
                {
                    runtimeData.flowNodes.Add(curFlowNode);
                    string portKey = curFlowNode.GetPortKey("Next");
                    if (runtimeData.outEdges.TryGetValue(portKey, out var edgeDatas))
                    {
                        foreach (var edgeData in edgeDatas)
                        {
                            if (runtimeData.nodes.TryGetValue(edgeData.inputNodeGuid, out var nodeData))
                            {
                                if (nodeData is FlowNode flowNode)
                                {
                                    nextFlowNodes.Add(flowNode);
                                }
                            }
                        }
                    }
                }

                curFlowNodes = nextFlowNodes;
            }

            return runtimeData;
        }

        public String GetInputPortName(FieldInfo fieldInfo)
        {
            return (fieldInfo.GetCustomAttribute(typeof(InputAttribute)) as InputAttribute)?.name;
        }

        public virtual void ProcessFlow()
        {
            foreach (var flowNode in flowNodes)
            {
                var inputFields = flowNode.GetInputFields();
                foreach (var inputField in inputFields)
                {
                    var inputPortKey = flowNode.GetPortKey(GetInputPortName(inputField));
                    if (inEdges.TryGetValue(inputPortKey, out var edgeData))
                    {
                        if (nodes.TryGetValue(edgeData.outputNodeGuid, out var nodeData))
                        {
                            var outputField = nodeData.GetOutputFieldByPortName(edgeData.outputPortName);
                            if (outputField == null)
                            {
                                continue;
                            }

                            inputField.SetValue(flowNode, outputField.GetValue(nodeData));
                        }
                    }
                }

                flowNode.Process();
            }
        }
    }
}