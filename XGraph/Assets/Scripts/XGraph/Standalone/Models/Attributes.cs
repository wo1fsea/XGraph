using System;

namespace XGraph
{
    public class XAttribute : Attribute
    {
        public string name;
        public bool multipleConnect;
        protected XAttribute(string name, bool multipleConnect = false)
        {
            this.name = name;
            this.multipleConnect = multipleConnect;
        }
    }

    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class InputAttribute : XAttribute
    {
        public InputAttribute(string name, bool multipleConnect = false) : base(name, multipleConnect)
        {
        }
    }

    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class OutputAttribute : XAttribute
    {
        public OutputAttribute(string name, bool multipleConnect = true) : base(name, multipleConnect)
        {
        }
    } 
    
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class PropertyAttribute : Attribute
    {
        public string name;
        public PropertyAttribute(string name)
        {
            this.name = name;
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