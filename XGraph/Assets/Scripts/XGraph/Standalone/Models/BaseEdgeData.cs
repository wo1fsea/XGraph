using System;

namespace XGraph
{
    [Serializable]
    public class BaseEdgeData
    {
        public string outputNodeGuid; // 输出节点的唯一标识符
        public string outputPortName; // 输出端口的名称
        public string inputNodeGuid;  // 输入节点的唯一标识符
        public string inputPortName;  // 输入端口的名称
    }
}