using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine.UIElements;

namespace XGraph.Editor
{
    public static class NodeProvider
    {
        private static List<Type> _nodeTypes = new List<Type>();

        static NodeProvider()
        {
            BuildNodeCache();
        }

        static void BuildNodeCache()
        {
            foreach (var nodeType in TypeCache.GetTypesDerivedFrom<BaseNodeData>())
            {
                if (!IsNodeAccessibleFromMenu(nodeType))
                    continue;
                
                _nodeTypes.Add(nodeType);
            }
        }

        static bool IsNodeAccessibleFromMenu(Type nodeType)
        {
            var nodeAttribute = nodeType.GetCustomAttribute<NodeMenuItemAttribute>();

            if (nodeAttribute == null)
            {
                return false;
            }

            return true;
        }
        
        public static List<Tuple<string, Type>> GetCompatibleNodes(BaseGraphData graphData)
        {
            var menu = new List<Tuple<string, Type>>();
            
            foreach (var nodeType in _nodeTypes)
            {
                var nodeAttribute = nodeType.GetCustomAttribute<NodeMenuItemAttribute>();
                menu.Add(new Tuple<string, Type>(nodeAttribute.name, nodeType));
            }
            
            return menu;
        }
    }
}