using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;
using UnityEngine;

namespace XGraph
{
    public class GraphViewWindow : EditorWindow
    {
        private BaseGraphView _graphView;
        public BaseGraphView GraphView => _graphView;

        [MenuItem("XGraph/Show Window")]
        public static void ShowWindow()
        {
            GraphViewWindow window = GetWindow<GraphViewWindow>();
            window.titleContent = new GUIContent("GraphView Window");
        }

        private void OnEnable()
        {
            ConstructGraphView();
        }

        private void OnDisable()
        {
            rootVisualElement.Remove(_graphView);
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
                string path = EditorUtility.SaveFilePanel("Save Graph", "Assets", "myGraph", "json");
                if (!string.IsNullOrEmpty(path))
                {
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

            // 将按钮容器添加到窗口的根视觉元素
            rootVisualElement.Add(buttonContainer);
        }
    }
}