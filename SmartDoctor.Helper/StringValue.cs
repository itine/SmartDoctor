using System;

namespace SmartDoctor.Helper
{
    public class StringValue: Attribute
    {
        public string Value { get; protected set; }

        public StringValue(string value)
        {
            Value = value;
        }
    }

}
