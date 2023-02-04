using System;

namespace Undebugger
{
    public class UndebuggerPriorityAttribute : Attribute
    {
        public readonly int Priority;

        public UndebuggerPriorityAttribute(int priority)
        {
            Priority = priority;
        }
    }
}
