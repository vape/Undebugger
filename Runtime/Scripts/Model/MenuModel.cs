using Deszz.Undebugger.Model.Commands;
using Deszz.Undebugger.Model.Status;
using System.Collections.Generic;

namespace Deszz.Undebugger.Model
{
    public class MenuModel
    {
        public CommandsGroupModel Commands = new CommandsGroupModel();
        public StatusGroupModel Status = new StatusGroupModel();
    }
}
