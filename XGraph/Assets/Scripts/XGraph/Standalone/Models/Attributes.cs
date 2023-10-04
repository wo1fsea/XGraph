using System;

namespace XGraph
{
    public class XAttribute : Attribute
    {
        public string name;
        public string displayName;
        public bool multipleConnect;
        public int showOrder;

        protected XAttribute(string name, string displayName = null, bool multipleConnect = false, int showOrder = 0)
        {
            this.name = name;
            this.displayName = displayName ?? name;
            this.multipleConnect = multipleConnect;
            this.showOrder = showOrder;
        }
    }

    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public sealed class InputAttribute : XAttribute
    {
        public InputAttribute(string name, string displayName = null, bool multipleConnect = false, int showOrder = 0) : base(name, displayName,
            multipleConnect, showOrder)
        {
        }
    }

    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public sealed class OutputAttribute : XAttribute
    {
        public OutputAttribute(string name, string displayName = null, bool multipleConnect = true, int showOrder = 0) : base(name, displayName,
            multipleConnect, showOrder)
        {
        }
    }

    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public sealed class PropertyAttribute : XAttribute
    {
        public PropertyAttribute(string name,string displayName = null, int showOrder = 0) : base(name, displayName, false, showOrder)
        {
        }
    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public sealed class NodeMenuItemAttribute : Attribute
    {
        public string name;

        public NodeMenuItemAttribute(string name)
        {
            this.name = name;
        }
    }
    
    [AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
    public sealed class EnumDisplayNameAttribute : Attribute
    {
        public readonly string description;

        public EnumDisplayNameAttribute(string description)
        {
            this.description = description;
        }
    }
}