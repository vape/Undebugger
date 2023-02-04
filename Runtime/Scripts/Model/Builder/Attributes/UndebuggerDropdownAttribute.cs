using System;

namespace Undebugger
{
    [AttributeUsage(AttributeTargets.Property)]
    public class UndebuggerDropdownAttribute : Attribute
    {
        public readonly object[] Values;

        public UndebuggerDropdownAttribute(object[] values)
        {
            Values = values;
        }
    }
}
