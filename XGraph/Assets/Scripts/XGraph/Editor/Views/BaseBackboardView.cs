using UnityEngine;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;

namespace XGraph
{
    public class BaseBackboardView: Blackboard
    {
        public BaseBackboardView(BaseGraphView graphView): base(graphView)
        {
            scrollable = true;
            addItemRequested += OnBlackboardAddItemRequested;
        }
        
        private void OnBlackboardAddItemRequested(Blackboard blackboard)
        {
            var menu = new GenericMenu();
            menu.AddItem(new GUIContent("Exposed Property"), false, () =>
            {
                // var property = new ExposedProperty();
                // GraphData.exposedProperties.Add(property);
                var item = new BlackboardRow(new BlackboardField { text = "String", typeText = "string" }, new Vector3Field());
                // item.RegisterCallback<MouseDownEvent>(e =>
                // {
                //     if (e.clickCount == 2 && e.button == 0)
                //     {
                //         var textField = new TextField {value = item.text};
                //         textField.RegisterCallback<FocusOutEvent>(evt =>
                //         {
                //             item.text = textField.value;
                //             GraphData.exposedProperties[item.text].propertyName = textField.value;
                //             blackboard.Remove(textField);
                //         });
                //         item.Add(textField);
                //         textField.Focus();
                //     }
                // });
                blackboard.Add(item);
            });
            menu.ShowAsContext();   
        }
    }
}