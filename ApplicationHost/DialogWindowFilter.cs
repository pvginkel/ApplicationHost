using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ApplicationHost
{
    internal class DialogWindowFilter : WindowFilter
    {
        public override WindowStyle WindowStyle
        {
            get { return WindowStyle.Caption; }
        }

        public override void ResizeWindow(AppHost host, IntPtr hWnd)
        {
            CenterWindow(hWnd, host.Size);
        }
    }
}
