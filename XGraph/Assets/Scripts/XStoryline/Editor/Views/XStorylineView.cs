using System;
using XGraph;

namespace XStoryline
{
    public class XStorylineView: BaseGraphView
    {
        public override Type GraphDataType => typeof(XStorylineGraph);
    }
}