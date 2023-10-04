using System;
using XGraph;

namespace XStoryline
{
    public enum CompareType 
    {
        [EnumDisplayName("==")]
        Equal,
        [EnumDisplayName("!=")]
        NotEqual,
        [EnumDisplayName(">")]
        Greater,
        [EnumDisplayName("<")]
        Less,
        [EnumDisplayName(">=")]
        GreaterOrEqual,
        [EnumDisplayName("<=")]
        LessOrEqual
    }
    
    [Serializable, NodeMenuItem("Control/Comparing")]
    public class ControlNode : FlowNode 
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
    
    [Serializable, NodeMenuItem("Control/If")]
    public class IfNode : ConditionalFlowNode
    {
    }
}