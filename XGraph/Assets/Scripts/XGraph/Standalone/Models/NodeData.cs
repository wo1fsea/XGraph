using System;

namespace XGraph
{
    [Serializable]
    public class BaseNodeData
    {
        public string title;
        public float x;
        public float y;
        public string guid;
        public string nodeType; // "NodeData1" or "NodeData2"
        
        public BaseNodeData()
        {
            guid = Guid.NewGuid().ToString();
            title = "New Node";
        }
    }

    [Serializable]
    public class NodeData1 : BaseNodeData
    {
        public string stringProp;
    }

    [Serializable]
    public class NodeData2 : BaseNodeData
    {
        public int intProp;
    }

    [Serializable]
    public class EdgeData
    {
        public string outputNodeGuid; // 输出节点的唯一标识符
        public string outputPortName; // 输出端口的名称
        public string inputNodeGuid;  // 输入节点的唯一标识符
        public string inputPortName;  // 输入端口的名称
    }
}