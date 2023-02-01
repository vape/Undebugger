using System;

namespace Undebugger
{
    [AttributeUsage(AttributeTargets.Class)]
    public class DebugTargetAttribute : Attribute
    {
        public string CommandsPage
        { get; set; }
        public int CommandsPagePriority
        { get; set; }

        public DebugTargetAttribute()
        { }

        public DebugTargetAttribute(string commandsPage, int commandsPagePriority)
        {
            CommandsPage = commandsPage;
            CommandsPagePriority = commandsPagePriority;
        }
    }
}
