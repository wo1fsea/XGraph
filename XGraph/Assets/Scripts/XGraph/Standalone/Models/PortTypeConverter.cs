using System;
using System.Collections.Generic;

namespace XGraph
{
    public class ITypeConverter
    {
        public virtual bool CanConvert(Type sourceType, Type targetType)
        {
            return false;
        }

        public virtual object Convert(object value, Type sourceType, Type targetType)
        {
            return null;
        }
    }
    
    public class PortTypeNumConverter: ITypeConverter
    {
        HashSet<Type> compatibleTypes = new(){ typeof(double), typeof(float), typeof(int), typeof(bool) }; 
        public override bool CanConvert(Type sourceType, Type targetType)
        {
            return compatibleTypes.Contains(sourceType) && compatibleTypes.Contains(targetType); 
        }

        public override object Convert(object value, Type sourceType, Type targetType)
        {
            return System.Convert.ChangeType(value, targetType);
        }
    }
    
    public class PortTypeToStringConverter: ITypeConverter
    {
        public override bool CanConvert(Type sourceType, Type targetType)
        {
            return targetType == typeof(string); 
        }

        public override object Convert(object value, Type sourceType, Type targetType)
        {
            if (value == null || targetType != typeof(string))
            {
                return null;
            }
            
            return value.ToString();
        }
    }

    public static class PortTypeConverter
    {
        public static List<ITypeConverter> typeConverters = new()
        {
            new PortTypeNumConverter(),
            new PortTypeToStringConverter()
        };

        public static void AddConverter(ITypeConverter converter)
        {
            typeConverters.Add(converter);
        }

        public static bool CanConvert(Type type1, Type type2)
        {
            if (type1 == type2)
            {
                return true;
            }
            
            foreach (var converter in typeConverters)
            {
                if (converter.CanConvert(type1, type2))
                {
                    return true;
                }
            }

            return false;
        }
        
        public static object Convert(object value, Type type1, Type type2)
        {
            if (type1 == type2)
            {
                return value;
            }
            
            foreach (var converter in typeConverters)
            {
                if (converter.CanConvert(type1, type2))
                {
                    return converter.Convert(value, type1, type2);
                }
            }
            return null;
        }
    
    }
}