using System.Collections.Generic;

namespace Undebugger.Model.Commands
{
    public class SegmentModel : IPrioritized
    {
        public int Priority
        { get; set; }

        public string Name;
        public List<CommandModel> Commands = new List<CommandModel>();
    }
}
