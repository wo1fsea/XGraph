using System;
using XGraph;

namespace XStoryline
{
    [Serializable, NodeMenuItem("Test/IntAdd")]
    public class IntAddNode : BlockingProcessFlowNode
    {
        [Input("Input1")] public int input1;
        [Input("Input2")] public int input2;
        [Output("Output")] public int output;

        public override void Process()
        {
            output = input1 + input2;
        }
    }

    [Serializable, NodeMenuItem("Test/DebugPrint")]
    public class IntPrintNode : BlockingProcessFlowNode
    {
        [Input("Message")] public string input;

        public override void Process()
        {
            XGraphDebuger.Log(input);
        }
    }

    [Serializable, NodeMenuItem("Test/If")]
    public class IfNode : ConditionalFlowNode
    {
    }
  
    public enum CompareType 
    {
        Equal,
        NotEqual,
        Greater,
        Less,
        GreaterOrEqual,
        LessOrEqual
    }
    [Serializable, NodeMenuItem("Test/Compare")]
    public class ComparingNode : FlowNode 
    {
        [Input("Previous", showOrder:-255), NonSerialized] public FlowLink previousNode;
        
        [Output("True", showOrder:-255), NonSerialized] public FlowLink trueNodes;
        [Output("False", showOrder:-254), NonSerialized] public FlowLink falseNodes;
        
        [Input("Input1", showOrder:0)]
        public float input1;
        
        [Property("CompareType", showOrder:1)] 
        public CompareType compareType;
        
        [Input("Input2", showOrder:2)]
        public float input2;
        
        public override string GetNextNodesPortKey()
        {
            bool result = false;
            switch (compareType)
            {
               case CompareType.Equal:
                   result = input1 == input2;
                   break;
               case CompareType.NotEqual:
                   result = input1 != input2;
                   break;
               case CompareType.Greater:
                   result = input1 > input2;
                   break;
               case CompareType.Less:
                   result = input1 < input2;
                   break;
               case CompareType.GreaterOrEqual:
                   result = input1 >= input2;
                   break;
               case CompareType.LessOrEqual:
                   result = input1 <= input2;
                   break;
            }
            
            return result ? GetPortKey("True") : GetPortKey("False");
        }
    } 
}