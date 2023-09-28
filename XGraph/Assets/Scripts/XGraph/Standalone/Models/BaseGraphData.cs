using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace XGraph
{
    [Serializable]
    public class BaseGraphData
    {
        public string graphName;

        public List<BaseNodeData> nodes = new List<BaseNodeData>();
        public List<BaseEdgeData> edges = new List<BaseEdgeData>();
        
        public List<StickyNoteData> stickyNotes = new List<StickyNoteData>();

        public BaseGraphData(string graphName)
        {
            this.graphName = graphName;
        }
        
        public static BaseGraphData CreateFromJson(string json)
        {
            return JsonConvert.DeserializeObject<BaseGraphData>(json, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto
            });
        }
        
        public string ToJson()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto
            });
        }
    }
}