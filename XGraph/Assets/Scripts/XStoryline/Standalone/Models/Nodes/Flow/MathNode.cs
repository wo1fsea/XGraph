using System;
using XGraph;

namespace XStoryline
{
    [Serializable, NodeMenuItem("Math/Add")]
    public class AddNode : BlockingProcessFlowNode
    {
        [Input("Input1")] public double input1;
        [Input("Input2")] public double input2;
        [Output("Output")] public double output;

        public override void Process()
        {
            output = input1 + input2;
        }
    }
    
    [Serializable, NodeMenuItem("Math/Sub")]
    public class SubNode : BlockingProcessFlowNode
    {
        [Input("Input1")] public double input1;
        [Input("Input2")] public double input2;
        [Output("Output")] public double output;

        public override void Process()
        {
            output = input1 - input2;
        }
    }
    
    [Serializable, NodeMenuItem("Math/Mul")]
    public class MulNode : BlockingProcessFlowNode
    {
        [Input("Input1")] public double input1;
        [Input("Input2")] public double input2;
        [Output("Output")] public double output;

        public override void Process()
        {
            output = input1 * input2;
        }
    }
   
    [Serializable, NodeMenuItem("Math/Div")]
    public class DivNode : BlockingProcessFlowNode
    {
        [Input("Input1")] public double input1;
        [Input("Input2")] public double input2;
        [Output("Output")] public double output;

        public override void Process()
        {
            output = input1 / input2;
        }
    }
}