using System;

namespace XGraph
{
    [Serializable, NodeMenuItem("test/node1")]
    public class NodeData1 : BaseNodeData
    {
        [Property("Input string")]
        public string stringProp;
        
        [Input("Input int")]
        public int intInput;
    }

    [Serializable, NodeMenuItem("test/node2")]
    public class NodeData2 : BaseNodeData
    {
        [Property("Input int")]
        public int intProp;
        
        [Output("Output string")]
        public string stringOutput;
        
        [Output("Output int")]
        public int intOutput;
    }
}