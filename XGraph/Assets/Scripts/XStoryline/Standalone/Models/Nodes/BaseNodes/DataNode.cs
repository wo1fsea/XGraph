using System;
using XGraph;

namespace XStoryline
{
    public class DataNode<T>: XStorylineNode
    {
        [Output("Output")]
        [Property("Data")]
        public T data;
    }
}