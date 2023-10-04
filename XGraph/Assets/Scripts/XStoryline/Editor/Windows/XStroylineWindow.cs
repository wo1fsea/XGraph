using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using XGraph;

namespace XStoryline
{
    public class XStroylineWindow: BaseGraphViewWindow
    {
        [MenuItem("XStoryline/Show Window")]
        public new static void ShowWindow()
        {
            BaseGraphViewWindow window = GetWindow<XStroylineWindow>();
            window.titleContent = new GUIContent("XStoryline");
        }

        protected override List<Tuple<string, Action>> GetToolbarButtons()
        {
            Action RunGraph = () =>
            {
                GraphView.GraphData.Run();
            };

            return new List<Tuple<string, Action>>()
            {
                new ("New", GraphView.OnNew),
                new ("Load", GraphView.OnLoad),
                new ("Save", GraphView.OnSave),
                new ("Save As...", GraphView.OnSaveAs),
                new ("Run", RunGraph),
            };
        }
    }
}