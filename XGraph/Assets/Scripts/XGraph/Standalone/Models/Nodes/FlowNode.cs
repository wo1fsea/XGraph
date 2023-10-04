using System;
using System.Collections.Generic;

namespace XGraph
{
    [Serializable]
    public abstract class FlowNode : BaseNodeData
    {
        public virtual void Process()
        {
        }
        
        public virtual string GetNextNodesPortKey()
        {
            return GetPortKey("Next"); 
        }
    }

    [Serializable]
    public abstract class StartFlowNode : FlowNode
    {
        [Output("Next", showOrder:-255), NonSerialized] public FlowLink nextNodes;
    }

    [Serializable]
    public abstract class EndFlowNode : FlowNode
    {
        [Input("Previous", showOrder:-255), NonSerialized] public FlowLink previousNode;
    }

    [Serializable]
    public abstract class ProcessFlowNode : FlowNode
    {
        [Input("Previous", showOrder:-255), NonSerialized] public FlowLink previousNode;
        [Output("Next", showOrder:-255), NonSerialized] public FlowLink nextNodes;
        
        public override void Process()
        {
            ProcessFinished();
        }

        protected void ProcessFinished()
        {
            onProcessFinished.Invoke(this);
        }
        
        [NonSerialized]
        public Action<ProcessFlowNode> onProcessFinished;
    }

    [Serializable]
    public abstract class NonBlockingProcessFlowNode : ProcessFlowNode
    {
    }

    [Serializable]
    public abstract class BlockingProcessFlowNode : ProcessFlowNode
    {
    }
    
    [Serializable]
    public abstract class ConditionalFlowNode: FlowNode
    {
        [Input("Previous", showOrder:-255), NonSerialized] public FlowLink previousNode;
        
        [Output("True", showOrder:-255), NonSerialized] public FlowLink trueNodes;
        [Output("False", showOrder:-254), NonSerialized] public FlowLink falseNodes;
        
        [Input("Condition")] public bool condition;
        
        public override string GetNextNodesPortKey()
        {
            return condition ? GetPortKey("True") : GetPortKey("False");
        }
    }
}