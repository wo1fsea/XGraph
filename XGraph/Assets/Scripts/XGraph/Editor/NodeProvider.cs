using UnityEditor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace XGraph.Editor
{
    public static class NodeProvider
    {
        private static List<Type> _nodeTypes = new();

        static NodeProvider()
        {
            BuildNodeCache();
        }

        private static void BuildNodeCache()
        {
            foreach (var nodeType in TypeCache.GetTypesDerivedFrom<BaseNodeData>().Where(IsNodeAccessibleFromMenu))
            {
                _nodeTypes.Add(nodeType);
            }
        }

        private static bool IsNodeAccessibleFromMenu(Type nodeType)
        {
            var nodeAttribute = nodeType.GetCustomAttribute<NodeMenuItemAttribute>();
            return nodeAttribute != null;
        }
        
        public static List<Tuple<string, Type>> GetCompatibleNodes(BaseGraphData graphData)
        {
            return (from nodeType in _nodeTypes let nodeAttribute = nodeType.GetCustomAttribute<NodeMenuItemAttribute>() select new Tuple<string, Type>(nodeAttribute.name, nodeType)).ToList();
        }
    }
}