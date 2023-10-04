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

        public virtual void Run()
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

        public List<FlowNode> startFlowNodes = new();

        public static BaseGraphRuntimeData ConstructFromGraphData(BaseGraphData graphData)
        {
            BaseGraphRuntimeData runtimeData = new BaseGraphRuntimeData();
            foreach (var nodeData in graphData.nodes)
            {
                var copiedNodeData = nodeData.Clone(runtimeData);
                runtimeData.nodes.Add(copiedNodeData.guid, copiedNodeData);

                if (copiedNodeData is StartFlowNode flowNode)
                {
                    runtimeData.startFlowNodes.Add(flowNode);
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

            return runtimeData;
        }

        public String GetInputPortName(FieldInfo fieldInfo)
        {
            return (fieldInfo.GetCustomAttribute(typeof(InputAttribute)) as InputAttribute)?.name;
        }

        void PullInputFiledData(FlowNode flowNode)
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
                        
                        var outputValue = outputField.GetValue(nodeData);
                        outputValue = PortTypeConverter.Convert(outputValue, outputField.FieldType, inputField.FieldType);
                        
                        inputField.SetValue(flowNode, outputValue);
                    }
                }
            }
        }
        
        void GetNextFlowNodes(FlowNode flowNode, List<FlowNode> nextFlowNodes)
        {
            string portKey = flowNode.GetNextNodesPortKey();
            if (outEdges.TryGetValue(portKey, out var edgeDatas))
            {
                foreach (var edgeData in edgeDatas)
                {
                    if (nodes.TryGetValue(edgeData.inputNodeGuid, out var nodeData))
                    {
                        if (nodeData is FlowNode nextFlowNode)
                        {
                            nextFlowNodes.Add(nextFlowNode);
                        }
                    }
                }
            }
        }

        public virtual void ProcessFlow()
        {
            List<FlowNode> curFlowNodes = new();
            curFlowNodes.AddRange(startFlowNodes);
            
            while (curFlowNodes.Count != 0)
            {
                List<FlowNode> nextFlowNodes = new();

                foreach (var curFlowNode in curFlowNodes)
                {
                    PullInputFiledData(curFlowNode);
                    curFlowNode.Process();
                    GetNextFlowNodes(curFlowNode, nextFlowNodes);
                }
                
                curFlowNodes = nextFlowNodes;
            }
            
        }
    }
}