using System;
using System.Linq;

namespace SmartDoctor.Helper
{
    public static class Extension
    {
        public static string GetStringValue(this Enum value)
        {
            var stringValue = value.ToString();
            var type = value.GetType();
            var fieldInfo = type.GetField(value.ToString());
            var attrs = fieldInfo.GetCustomAttributes(typeof(StringValue), false) as StringValue[];
            if (attrs.Any())
                stringValue = attrs.First().Value;
            return stringValue;
        }
    }
}
