using System;
using XGraph;

namespace XStoryline
{

    [Serializable]
    public abstract class FlowNode : XStorylineNode {
       
    }

    [Serializable]
    public abstract class StartFlowNode : FlowNode
    {
        [Output("Next"), NonSerialized]
        public FlowLink nextNodes;
    }

    [Serializable]
    public abstract class EndFlowNode : FlowNode
    {
        [Input("Previous"), NonSerialized]
        public FlowLink previousNode;
    }

    [Serializable]
    public abstract class InternalFlowNode : FlowNode
    {
        [Input("Previous"), NonSerialized]
        public FlowLink previousNode;

        [Output("Next"), NonSerialized]
        public FlowLink nextNodes;
    }

    [Serializable]
    public abstract class NonBlockingFlowNode : InternalFlowNode
    {


    }

    [Serializable]
    public abstract class BlockingFlowNode : InternalFlowNode
    {
        protected void ProcessFinished()
        {
            onProcessFinished.Invoke(this);
        }
        
        public Action<BlockingFlowNode> onProcessFinished;
    }
}