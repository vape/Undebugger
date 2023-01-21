using System.Collections.Generic;

namespace Undebugger.Model
{
    public interface IPrioritized
    {
        int Priority
        { get; }
    }

    public class PriorityComparer : IComparer<IPrioritized>
    {
        public static readonly PriorityComparer Instance = new PriorityComparer();

        public int Compare(IPrioritized x, IPrioritized y)
        {
            return y.Priority.CompareTo(x.Priority);
        }
    }
}
