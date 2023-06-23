

namespace XGraph
{
    [System.Serializable]
    public class NodeData
    {
        public string title;
        public float x, y;
        public string guid;  // 唯一标识符，用于连接的参考
    }

    [System.Serializable]
    public class EdgeData
    {
        public string outputNodeGuid; // 输出节点的唯一标识符
        public string outputPortName; // 输出端口的名称
        public string inputNodeGuid;  // 输入节点的唯一标识符
        public string inputPortName;  // 输入端口的名称
    }
}