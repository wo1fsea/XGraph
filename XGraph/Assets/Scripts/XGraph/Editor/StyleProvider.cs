using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace XGraph
{
    public static class StyleProvider
    {
        private static readonly Dictionary<string, string> StyleMap = new()
        {
            {"grid-background", "StyleSheets/GridBackground"},
        };

        public static void SetupStyle(VisualElement element, string className)
        {
            if (!StyleMap.TryGetValue(className, out var styleSheetPath)) return;
            
            element.styleSheets.Add(Resources.Load<StyleSheet>(styleSheetPath));
            element.AddToClassList(className);
        }
    }
}