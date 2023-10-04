using System;
using System.Linq;
using System.Numerics;
using System.Reflection;

namespace XGraph
{
    [Serializable]
    public class BaseNodeData
    {
        // for editor
        public Vector2 editorPosition;
        
        // for runtime
        [NonSerialized]
        public BaseGraphRuntimeData runtimeContext;

        public string guid = Guid.NewGuid().ToString();
        public virtual string Title => GetType().ToString().Split(".")[^1].Replace("Node", "");

        public FieldInfo[] GetFields()
        {
            return GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        }
        
        public FieldInfo[] GetOrderedFields()
        {
            return GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).OrderBy(
                field =>
                    (field.GetCustomAttributes(typeof(XAttribute)).FirstOrDefault() as XAttribute)?.showOrder).ToArray();
        }

        public FieldInfo[] GetInputFields()
        {
            return GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).Where(
                field =>
                    field.GetCustomAttribute(typeof(InputAttribute)) != null).ToArray();
        }

        public FieldInfo[] GetOutputFields()
        {
            return GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).Where(
                field =>
                    field.GetCustomAttribute(typeof(OutputAttribute)) != null).ToArray();
        }

        public FieldInfo GetInputFieldByPortName(String portName)
        {
            var fields = GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                .Where(
                    field =>
                        (field.GetCustomAttribute(typeof(InputAttribute)) as InputAttribute)?.name == portName);
            return fields.FirstOrDefault();
        }

        public FieldInfo GetOutputFieldByPortName(String portName)
        {
            var fields = GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                .Where(
                    field =>
                        (field.GetCustomAttribute(typeof(OutputAttribute)) as OutputAttribute)?.name == portName);
            return fields.FirstOrDefault();
        }

        public string GetPortKey(string portName)
        {
            return $"{guid}:{portName}";
        }

        public bool IsPortOutput(string portName)
        {
            return GetType().GetField(portName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                ?.GetCustomAttribute(typeof(OutputAttribute)) != null;
        }

        public bool IsPortInput(string portName)
        {
            return GetType().GetField(portName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                ?.GetCustomAttribute(typeof(InputAttribute)) != null;
        }
        
        public BaseNodeData Clone(BaseGraphRuntimeData runtimeData)
        {
            var data = MemberwiseClone() as BaseNodeData;
            if (data == null) throw new ArgumentNullException(nameof(data));
            data.runtimeContext = runtimeData;
            return data;
        }
    }

    [Serializable]
    public class StickyNoteData : BaseNodeData
    {
        public float width = 200;
        public float height = 100;
        public string title;
        public string content;
    }
}