using System.Collections.Generic;

namespace Deszz.Undebugger.Model.Status
{
    public class StatusGroupModel : IGroupModel
    {
        public List<IStatusSegmentDriver> Segments = new List<IStatusSegmentDriver>();

        public void Sort()
        {
            Segments.Sort(PriorityComparer.Instance);
        }
    }
}
