using System;
using XGraph;

namespace XStoryline
{
    [Serializable, NodeMenuItem("Flow/End")]
    public class EndNode : EndFlowNode
    {
        public override void Process()
        {
            XGraphDebuger.Log("EndNode");
        }
    }
}