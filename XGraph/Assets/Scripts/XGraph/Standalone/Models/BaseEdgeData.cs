using System;

namespace XGraph
{
    [Serializable]
    public class BaseEdgeData
    {
        public string outputNodeGuid; 
        public string outputPortName; 
        public string inputNodeGuid;  
        public string inputPortName;  
        
        public string GetOutputPortKey()
        {
            return $"{outputNodeGuid}:{outputPortName}";
        }
        
        public string GetInputPortKey()
        {
            return $"{inputNodeGuid}:{inputPortName}";
        }
    }
}