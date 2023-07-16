using UnityEditor.Experimental.GraphView;

namespace XGraph
{
    public class BaseEdgeView: Edge
    {
        private BaseEdgeData _edgeData;
        public BaseEdgeData EdgeData => _edgeData;
        
        public BaseEdgeView(BaseEdgeData edgeData)
        {
            _edgeData = edgeData;
        }
    }
}