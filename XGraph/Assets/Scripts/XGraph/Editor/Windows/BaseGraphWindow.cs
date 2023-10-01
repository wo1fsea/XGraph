using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace XGraph
{
    public class BaseGraphViewWindow : EditorWindow
    {
        private BaseGraphView _graphView;
        public BaseGraphView GraphView => _graphView;
        
        public string StyleSheetPath => "Node.uss";

        public static void ShowWindow()
        {
            BaseGraphViewWindow window = GetWindow<BaseGraphViewWindow>();
            window.titleContent = new GUIContent("GraphView Window");
        }

        private void OnEnable()
        {
            ConstructGraphView();
            
            XGraphDebuger.OnDebugLogEvent += Debug.Log;
            XGraphDebuger.OnDebugLogWarningEvent += Debug.LogWarning;
            XGraphDebuger.OnDebugLogErrorEvent += Debug.LogError;
        }

        private void OnDisable()
        {
            rootVisualElement.Remove(_graphView);
            
            XGraphDebuger.OnDebugLogEvent -= Debug.Log;
            XGraphDebuger.OnDebugLogWarningEvent -= Debug.LogWarning;
            XGraphDebuger.OnDebugLogErrorEvent -= Debug.LogError;
        }

        private void ConstructGraphView()
        {
            _graphView = new BaseGraphView();

            _graphView.StretchToParentSize();
            // 将GraphView添加到窗口的根视觉元素
            rootVisualElement.Add(_graphView); 
            
            // 创建一个容器来包含按钮
            var buttonContainer = new VisualElement();
            buttonContainer.style.flexDirection = FlexDirection.Row;
            buttonContainer.style.paddingTop = 5;
            buttonContainer.style.paddingLeft = 5;
            buttonContainer.style.paddingRight = 5;

            // 创建保存按钮并添加到容器中
            Button saveButton = new Button(() =>
            {
                string path = EditorUtility.SaveFilePanel("Save", "Assets", GraphView.GraphData.graphName, "json");
                if (!string.IsNullOrEmpty(path))
                {
                    var graphName = System.IO.Path.GetFileNameWithoutExtension(path);
                    GraphView.GraphData.graphName = graphName;
                    _graphView.SaveToFile(path);
                }
            });
            saveButton.text = "Save";
            buttonContainer.Add(saveButton);

            // 创建加载按钮并添加到容器中
            Button loadButton = new Button(() =>
            {
                string path = EditorUtility.OpenFilePanel("Select Graph file", "Assets", "json");
                if (!string.IsNullOrEmpty(path))
                {
                    _graphView.LoadFromFile(path);
                }
            });
            loadButton.text = "Load";
            buttonContainer.Add(loadButton);
            
            Button runButton = new Button(() =>
            {
                _graphView.GraphData.Run();
            });
            runButton.text = "Run";
            buttonContainer.Add(runButton);

            // 将按钮容器添加到窗口的根视觉元素
            rootVisualElement.Add(buttonContainer);
        }
    }
}