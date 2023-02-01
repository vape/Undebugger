using System.Collections.Generic;
using System.Reflection;

namespace Undebugger.Model.Builder
{
    internal struct TypeData
    {
        internal struct CommandsPageOverride
        {
            public bool Valid;
            public string Name;
            public int Priority;
        }

        public bool IsDebugTarget;
        public string Name;
        public CommandsPageOverride PageOverride;
        public List<MethodInfo> ActionMethods;
        public List<PropertyInfo> ToggleProperties;
        public List<MethodInfo> HandlerMethods;
    }
}
