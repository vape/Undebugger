using System;

namespace Undebugger
{
    [AttributeUsage(AttributeTargets.Method)]
    public class UndebuggerActionAttribute : Attribute
    { }
}
