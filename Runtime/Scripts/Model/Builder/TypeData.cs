using System;
using System.Collections.Generic;
using System.Reflection;

namespace Undebugger.Model.Builder
{
    internal enum MethodType
    {
        Action,
        Handler,
        TextAction,
        IntAction
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
        public UndebuggerDefaultValueAttribute DefaultValueAttribute;
        public MethodInfo Info;
    }

    internal struct PropertyData
    {
        public PropertyType Type;
        public UndebuggerNameAttribute NameAttribute;
        public UndebuggerPriorityAttribute PriorityAttribute;
        public UndebuggerDefaultValueAttribute DefaultValueAttribute;
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
