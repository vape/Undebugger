using System;
using System.Collections.Generic;
using System.Reflection;

namespace Undebugger.Model.Builder
{
    internal enum MethodType
    {
        Action,
        Handler
    }

    internal enum PropertyType
    {
        Toggle,
        Dropdown,
        Carousel
    }

    internal struct MethodData
    {
        public MethodType Type;
        public UndebuggerNameAttribute NameAttribute;
        public UndebuggerPriorityAttribute PriorityAttribute;
        public MethodInfo Info;
    }

    internal struct PropertyData
    {
        public PropertyType Type;
        public UndebuggerNameAttribute NameAttribute;
        public UndebuggerPriorityAttribute PriorityAttribute;
        public object[] Values;
        public PropertyInfo Info;
    }

    internal struct TypeData
    {
        public bool IsDebugTarget;
        public Type Type;
        public UndebuggerNameAttribute NameAttribute;
        public UndebuggerPriorityAttribute PriorityAttribute;
        public List<MethodData> Methods;
        public List<PropertyData> Properties;
    }
}
