using System;

namespace Undebugger
{
    public class UndebuggerNameAttribute : Attribute
    {
        public readonly string Name;

        public UndebuggerNameAttribute(string name)
        {
            Name = name;
        }
    }
}
