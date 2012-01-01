using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ApplicationHost.Win32;

namespace ApplicationHost
{
	partial class AppHost
	{
        private class WindowPreCallback : WindowHook<Win32.CallWndProcMessage>
        {
            private AppHost _host;

            public WindowPreCallback(AppHost host)
                : base(WindowHookType.CallWndProc)
            {
                if (host == null)
                    throw new ArgumentNullException("host");

                _host = host;
            }

            protected override void Callback(ref CallWndProcMessage msg)
            {
                switch (msg.Msg)
                {
                    case NativeMethods.WM_SHOWWINDOW:
                        if (msg.WParam != IntPtr.Zero)
                            _host.WindowShown(ref msg);
                        break;
                }
            }
        }

        private class WindowPostCallback : WindowHook<Win32.CallWndProcReturnMessage>
        {
            private AppHost _host;

            public WindowPostCallback(AppHost host)
                : base(WindowHookType.CallWndProcReturn)
            {
                if (host == null)
                    throw new ArgumentNullException("host");

                _host = host;
            }

            protected override void Callback(ref CallWndProcReturnMessage msg)
            {
                switch (msg.Msg)
                {
                    case NativeMethods.WM_DESTROY:
                        _host.WindowDestroyed(ref msg);
                        break;

                    case NativeMethods.WM_SIZE:
                        _host.WindowResized(ref msg);
                        break;
                }
            }
        }
    }
}
