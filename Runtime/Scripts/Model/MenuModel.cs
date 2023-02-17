using Undebugger.Model.Commands;
using Undebugger.Model.Status;

namespace Undebugger.Model
{
    public class MenuModel
    {
        public BuiltinGroup StartGroup = BuiltinGroup.Commands;
        public CommandsGroupModel Commands = new CommandsGroupModel();
        public StatusGroupModel Status = new StatusGroupModel();

        public void Sort()
        {
            Commands.Sort();
            Status.Sort();
        }
    }
}
