using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using ApplicationHost.Win32;

namespace ApplicationHost
{
    internal class MainWindowFilter : WindowFilter
    {
        private bool _maximize;

        public MainWindowFilter(bool maximize)
        {
            _maximize = maximize;
        }

        public override WindowStyle WindowStyle
        {
            get { return WindowStyle.Caption | WindowStyle.SizeFrame; }
        }

        public override WindowStyleEx WindowStyleEx
        {
            get { return WindowStyleEx.ApplicationWindow; }
        }

        public override void AttachWindow(AppHost host, IntPtr hWnd)
        {
            NativeMethods.SetParent(hWnd, GetHostHandle(host));
        }

        public override void ResizeWindow(AppHost host, IntPtr hWnd)
        {
            // Main windows aren't centered. We only handle the maximize case, and
            // only that when the window allows maximization.

            var style = (WindowStyle)NativeMethods.GetWindowLong(hWnd, NativeMethods.GWL_STYLE);

            if (_maximize && (style & WindowStyle.MaximizeBox) == WindowStyle.MaximizeBox)
            {
                if ((style & (WindowStyle.Caption | WindowStyle.SizeFrame)) != 0)
                {
                    // If we are going to maximize the main window, remove its
                    // chrome.

                    NativeMethods.SetWindowLong(
                        hWnd,
                        NativeMethods.GWL_STYLE,
                        (int)(style & ~(WindowStyle.Caption | WindowStyle.SizeFrame))
                    );

                    // Inform the window of the changes.

                    NativeMethods.SetWindowPos(
                        hWnd,
                        IntPtr.Zero,
                        0, 0, 0, 0,
                        NativeMethods.SWP_NOMOVE | NativeMethods.SWP_NOSIZE | NativeMethods.SWP_NOZORDER | NativeMethods.SWP_FRAMECHANGED
                    );
                }

                var parent = NativeMethods.GetParent(hWnd);
                var hostHandle = GetHostHandle(host);

                MaximizeWindow(hWnd, host.Size);
            }
        }
    }
}
