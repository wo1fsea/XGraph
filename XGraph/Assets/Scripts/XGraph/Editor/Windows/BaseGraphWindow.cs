using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace XGraph
{
    public class BaseGraphViewWindow : EditorWindow
    {
        public BaseGraphView GraphView { get; private set; }
        
        public VisualElement MainContainer { get; private set; }
        public VisualElement ToolbarContainer { get; private set; }
        
        public Label GraphNameLabel { get; private set; }
        
        public BaseGraphViewWindow()
        {
            XGraphDebuger.OnDebugLogEvent += Debug.Log;
            XGraphDebuger.OnDebugLogWarningEvent += Debug.LogWarning;
            XGraphDebuger.OnDebugLogErrorEvent += Debug.LogError;
        }
        
        private void OnDestroy()
        {
            XGraphDebuger.OnDebugLogEvent -= Debug.Log;
            XGraphDebuger.OnDebugLogWarningEvent -= Debug.LogWarning;
            XGraphDebuger.OnDebugLogErrorEvent -= Debug.LogError; 
        }
        
        public static void ShowWindow()
        {
            BaseGraphViewWindow window = GetWindow<BaseGraphViewWindow>();
            window.titleContent = new GUIContent("GraphView Window");
        }

        private void OnEnable()
        {
            ConstructGraphView();
        }

        public virtual BaseGraphView SetupGraphView()
        {
            return new BaseGraphView();
        }
        
        public virtual VisualElement SetupMainContainer()
        {
            var mainContainer = new VisualElement();
            mainContainer.StretchToParentSize();
            mainContainer.style.flexDirection = FlexDirection.Column;
            return mainContainer;
        }
        
        public virtual VisualElement SetupToolbarContainer()
        {
            var toolbarContainer = new VisualElement();
            toolbarContainer.style.flexDirection = FlexDirection.Row;
            toolbarContainer.style.paddingTop = 5;
            toolbarContainer.style.paddingLeft = 5;
            toolbarContainer.style.paddingRight = 5;
            toolbarContainer.style.paddingBottom = 5;
            toolbarContainer.style.backgroundColor = new Color(0.2f, 0.2f, 0.2f);
            toolbarContainer.style.alignItems = Align.Center; 
            return toolbarContainer;
        }

        protected virtual List<Tuple<string, Action>> GetToolbarButtons()
        {
            return new List<Tuple<string, Action>>();
        }

        private void ConstructGraphView()
        {
            GraphView = SetupGraphView();
            GraphView.style.flexGrow = 1;
            GraphView.style.flexShrink = 0;
            
            MainContainer = SetupMainContainer();
            ToolbarContainer = SetupToolbarContainer();

            rootVisualElement.Add(MainContainer);
            MainContainer.Add(ToolbarContainer);
            MainContainer.Add(GraphView);
            
            var toolbarButtons = GetToolbarButtons();
            foreach (var toolbarButton in toolbarButtons)
            {
                Button button = new Button(toolbarButton.Item2)
                {
                    text = toolbarButton.Item1
                };
                ToolbarContainer.Add(button);
            }
            
            var flexibleSpace = new VisualElement();
            flexibleSpace.style.flexGrow = 1;
            ToolbarContainer.Add(flexibleSpace);

            GraphNameLabel = new Label();
            ToolbarContainer.Add(GraphNameLabel);
        }
    }
}