using Deszz.Undebugger.Model.Commands;
using Deszz.Undebugger.Model.Status;

namespace Deszz.Undebugger.Model
{
    public class MenuModel
    {
        public CommandsGroupModel Commands = new CommandsGroupModel();
        public StatusGroupModel Status = new StatusGroupModel();

        public void Sort()
        {
            Commands.Sort();
            Status.Sort();
        }
    }
}
