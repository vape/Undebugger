using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Undebugger.UI.Layout
{
    internal interface ILayoutResizeHandler
    {
        void OnBeforeSizeChanged();
        void OnAfterSizeChanged();
    }
}
