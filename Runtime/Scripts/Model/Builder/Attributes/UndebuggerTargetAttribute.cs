using System;

namespace Undebugger
{
    [AttributeUsage(AttributeTargets.Class)]
    public class UndebuggerTargetAttribute : Attribute
    {
        public UndebuggerTargetAttribute()
        { }
    }
}
