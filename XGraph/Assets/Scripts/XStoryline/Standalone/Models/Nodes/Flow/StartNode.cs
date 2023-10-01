using System;
using XGraph;

namespace XStoryline 
{
    [Serializable, NodeMenuItem("Flow/Start")]
    public class StartNode : StartFlowNode
    {
        public override void Process() 
        {
            XGraphDebuger.Log("StartNode"); 
        }
    }
}