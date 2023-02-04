using System;

namespace Undebugger
{
    [AttributeUsage(AttributeTargets.Property)]
    public class UndebuggerToggleAttribute : Attribute
    { }
}
