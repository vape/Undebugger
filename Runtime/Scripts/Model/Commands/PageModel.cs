using System.Collections.Generic;

namespace Deszz.Undebugger.Model.Commands
{
    public class PageModel : IPrioritized
    {
        public int Priority
        { get; set; }

        public string Name;
        public List<SegmentModel> Segments = new List<SegmentModel>();
    }
}
