using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ApplicationHost
{
    internal class PopupWindowFilter : WindowFilter
    {
        //public override WindowStyle WindowStyle
        //{
        //    get { return WindowStyle.Popup; }
        //}

        public override WindowStyleEx WindowStyleEx
        {
            get { return WindowStyleEx.ApplicationWindow; }
        }

        public override void ResizeWindow(AppHost host, IntPtr hWnd)
        {
            CenterWindow(hWnd, host.Size);
        }
    }
}
