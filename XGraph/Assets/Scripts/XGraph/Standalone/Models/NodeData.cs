using System;

namespace XGraph
{
    [Serializable]
    public class BaseNodeData
    {
        public float x;
        public float y;
        public string guid;
        public virtual string Title => "Base Node";

        public BaseNodeData()
        {
            guid = Guid.NewGuid().ToString();
        }
    }




}