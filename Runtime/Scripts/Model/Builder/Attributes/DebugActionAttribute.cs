using System;

namespace Undebugger
{
    [AttributeUsage(AttributeTargets.Method)]
    public class DebugActionAttribute : Attribute
    { }
}
