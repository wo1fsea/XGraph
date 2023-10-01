using System;

namespace XGraph
{
    [Serializable]
    public abstract class FlowNode : BaseNodeData
    {
        public virtual void Process()
        {
        }
    }

    [Serializable]
    public abstract class StartFlowNode : FlowNode
    {
        [Output("Next"), NonSerialized] public FlowLink nextNodes;
    }

    [Serializable]
    public abstract class EndFlowNode : FlowNode
    {
        [Input("Previous"), NonSerialized] public FlowLink previousNode;
    }

    [Serializable]
    public abstract class InternalFlowNode : FlowNode
    {
        [Input("Previous"), NonSerialized] public FlowLink previousNode;

        [Output("Next"), NonSerialized] public FlowLink nextNodes;
    }

    [Serializable]
    public abstract class ProcessFlowNode : InternalFlowNode
    {
        public override void Process()
        {
            ProcessFinished();
        }

        protected void ProcessFinished()
        {
            onProcessFinished.Invoke(this);
        }

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
}