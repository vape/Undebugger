using System;

namespace Undebugger
{
    [AttributeUsage(AttributeTargets.Property)]
    public class UndebuggerCarouselAttribute : Attribute
    {
        public readonly object[] Values;

        public UndebuggerCarouselAttribute(object[] values)
        {
            Values = values;
        }
    }
}
