using System;

namespace XGraph
{
    [Serializable]
    public class BaseNodeData
    {
        public float x;
        public float y;
        public string guid;
        public virtual string Title => GetType().ToString().Split(".")[^1].Replace("Node", "");

        public BaseNodeData()
        {
            guid = Guid.NewGuid().ToString();
        }
    }

    [Serializable]
    public class StickyNoteData : BaseNodeData
    {
        public float width = 200;
        public float height = 100;
        public string title;
        public string content;
    }


}