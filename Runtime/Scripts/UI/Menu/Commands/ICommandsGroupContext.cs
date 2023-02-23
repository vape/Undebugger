using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Undebugger.UI.Menu.Commands
{
    public interface ICommandsGroupContext
    {
        void TryCloseOnAction();
    }
}
