using System;
using XGraph;

namespace XStoryline
{
    [Serializable, NodeMenuItem("Debug/DebugPrint")]
    public class DebugPrintNode : BlockingProcessFlowNode
    {
        [Input("Message")] public string input;

        public override void Process()
        {
            XGraphDebuger.Log(input);
        }
    }
}