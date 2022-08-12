using System.Collections.Generic;

namespace Deszz.Undebugger.Model.Commands
{
    public class SegmentModel
    {
        public string Name;
        public List<CommandModel> Commands = new List<CommandModel>();
    }
}
