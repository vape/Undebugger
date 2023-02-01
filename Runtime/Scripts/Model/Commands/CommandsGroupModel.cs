using System.Collections.Generic;

namespace Undebugger.Model.Commands
{
    public class CommandsGroupModel : IGroupModel
    {
        public const string GlobalPageName = "Global";
        public const int GlobalPagePriority = 1000;

        public List<PageModel> Pages = new List<PageModel>();

        public PageModel GetGlobalPage()
        {
            return FindOrCreatePage(GlobalPageName, GlobalPagePriority);
        }

        public PageModel FindOrCreatePage(string name, int priority)
        {
            for (int i = 0; i < Pages.Count; ++i)
            {
                if (Pages[i].Name == name)
                {
                    return Pages[i];
                }
            }

            var page = new PageModel()
            {
                Name = name,
                Priority = priority
            };

            Pages.Add(page);

            return page;
        }

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
