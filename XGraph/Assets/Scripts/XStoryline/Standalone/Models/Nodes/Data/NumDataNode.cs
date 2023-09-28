using System;
using XGraph;

namespace XStoryline
{
    [Serializable, NodeMenuItem("Data/Int")]
    public class IntDataNode : DataNode<int> {}
    
    [Serializable, NodeMenuItem("Data/Long")]
    public class LongDataNode: DataNode<long>{}
    
    [Serializable, NodeMenuItem("Data/Float")]
    public class FloatDataNode: DataNode<float>{}
    
    [Serializable, NodeMenuItem("Data/Double")]
    public class DoubleDataNode: DataNode<double>{}
    
}