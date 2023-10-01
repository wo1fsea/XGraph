using System;
using XGraph;

namespace XStoryline 
{
    [Serializable, NodeMenuItem("Flow/IntAdd")]
    public class IntAddNode : BlockingProcessFlowNode
    {
        [Input("Input1")]
        public int input1;

        [Input("Input2")]
        public int input2;

        [Output("Output")]
        public int output;

        public override void Process() 
        {
            output = input1 + input2;
        }
    } 
    
    [Serializable, NodeMenuItem("Flow/IntPrint")]
    public class IntPrintNode : BlockingProcessFlowNode
    {
        [Input("Input")]
        public int input;

        public override void Process() 
        {
            XGraphDebuger.Log(input.ToString());
        }
    }
}