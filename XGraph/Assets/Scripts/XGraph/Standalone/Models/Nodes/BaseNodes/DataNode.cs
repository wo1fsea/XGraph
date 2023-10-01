namespace XGraph
{
    public class DataNode<T>: BaseNodeData 
    {
        [Output("Output")]
        [Property("Data")]
        public T data;
    }
}