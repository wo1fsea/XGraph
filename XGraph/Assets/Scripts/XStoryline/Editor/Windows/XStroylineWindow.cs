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
            BaseGraphViewWindow window = GetWindow<BaseGraphViewWindow>();
            window.titleContent = new GUIContent("XStoryline Window");
        }
        
    }
}