using System.Collections.Generic;

namespace Deszz.Undebugger.Model.Commands
{
    public class CommandsGroupModel : IGroupModel
    {
        public List<PageModel> Pages = new List<PageModel>();

        public void Sort()
        {
            for (int i = 0; i < Pages.Count; ++i)
            {
                Pages[i].Segments.Sort(PriorityComparer.Instance);
            }

            Pages.Sort(PriorityComparer.Instance);
        }
    }
}
