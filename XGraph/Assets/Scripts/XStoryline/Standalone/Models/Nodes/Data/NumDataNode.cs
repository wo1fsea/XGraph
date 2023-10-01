using System;
using System.Numerics;
using XGraph;

namespace XStoryline
{
    [Serializable, NodeMenuItem("Data/Bool")]
    public class BoolNode : DataNode<bool> {}
    
    [Serializable, NodeMenuItem("Data/Int")]
    public class IntNode : DataNode<int> {}
    
    [Serializable, NodeMenuItem("Data/Float")]
    public class FloatNode: DataNode<float>{}
    
    [Serializable, NodeMenuItem("Data/Vector2")]
    public class Vector2Node: DataNode<Vector2>{}
   
    [Serializable, NodeMenuItem("Data/Vector3")]
    public class Vector3Node: DataNode<Vector3>{}
}