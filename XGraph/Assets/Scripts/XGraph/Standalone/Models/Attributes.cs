using System;

namespace XGraph
{
    public class XAttribute : Attribute
    {
        public string name;
        public bool multipleConnect;
        public int showOrder;
        
        protected XAttribute(string name, bool multipleConnect = false, int showOrder = 0)
        {
            this.name = name;
            this.multipleConnect = multipleConnect;
            this.showOrder = showOrder;
        }
    }

    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class InputAttribute : XAttribute
    {
        public InputAttribute(string name, bool multipleConnect = false, int showOrder = 0) : base(name, multipleConnect, showOrder)
        {
        }
    }

    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class OutputAttribute : XAttribute
    {
        public OutputAttribute(string name, bool multipleConnect = true, int showOrder = 0) : base(name, multipleConnect, showOrder)
        {
        }
    } 
    
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class PropertyAttribute : XAttribute
    {
        public PropertyAttribute(string name, int showOrder = 0) : base(name, false, showOrder)
        {
        }
    }
    
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class NodeMenuItemAttribute : Attribute
    {
        public string name;
        public NodeMenuItemAttribute(string name)
        {
            this.name = name;
        }
    }
}