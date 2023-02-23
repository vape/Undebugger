using System;

namespace Undebugger
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Method)]
    public class UndebuggerDefaultValueAttribute : Attribute
    {
        public readonly object Value;

        public UndebuggerDefaultValueAttribute(object value)
        {
            Value = value;
        }
    }
}
