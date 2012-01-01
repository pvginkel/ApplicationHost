using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using ApplicationHost.Win32;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace ApplicationHost
{
    /// <summary>
    /// Defines how a window created by the hosted application should be treated.
    /// Inherit this class when the application hosted in the application host
    /// has windows which aren't handled correctly by the default provided filters.
    /// </summary>
    public abstract class WindowFilter
    {
        private static IntPtr _desktopWindow;

        private IntPtr _hostHandle;

        internal IntPtr GetHostHandle(AppHost host)
        {
            if (_hostHandle == IntPtr.Zero)
            {
                if (host.InvokeRequired)
                {
                    return (IntPtr)host.Invoke(
                        new Func<AppHost, IntPtr>(GetHostHandle), host
                    );
                }
                else
                {
                    _hostHandle = host.Handle;
                }
            }

            return _hostHandle;
        }

        static WindowFilter()
        {
            _desktopWindow = NativeMethods.GetDesktopWindow();
        }

        /// <summary>
        /// Get the <see cref="ApplicationHost.WindowStyle"/>'s that must be matched for a window
        /// to be handled by the filter.
        /// </summary>
        public virtual WindowStyle WindowStyle
        {
            get { return 0; }
        }

        /// <summary>
        /// Get the <see cref="ApplicationHost.WindowStyleEx"/>'s that must be matched for
        /// a window to be handled by the filter. Defaults to <c>0</c>.
        /// </summary>
        public virtual WindowStyleEx WindowStyleEx
        {
            get { return 0; }
        }

        /// <summary>
        /// Get whether a windows must not be parented for a window to be
        /// handled by the filter. Defaults to <c>true</c>.
        /// </summary>
        public virtual bool RequireNotParented
        {
            get { return true; }
        }

        /// <summary>
        /// Called when the matched window must be resized. Override when
        /// windows handled by the filter must be resized based on the
        /// dimensions of the application host.
        /// </summary>
        /// <param name="host">Application host that hosts the application
        /// which created the provided window.</param>
        /// <param name="hWnd">Handle to the window to be resized.</param>
        public virtual void ResizeWindow(AppHost host, IntPtr hWnd)
        {
        }

        /// <summary>
        /// Returns <c>true</c> when the provided window matches the filter.
        /// The default implementation checks <see cref="RequireNotParented"/>,
        /// <see cref="WindowStyle"/> and <see cref="WindowStyleEx"/>.
        /// </summary>
        /// <param name="hWnd">Handle to the window to be matched.</param>
        /// <returns><c>true</c> when the window matches the filter; otherwise <c>false</c>.</returns>
        public virtual bool IsMatch(IntPtr hWnd)
        {
            // Sanity check for invalid filters.

            if (WindowStyle == 0 && WindowStyleEx == 0)
                return false;

            if (RequireNotParented && IsParented(hWnd))
                return false;

            if (WindowStyle != 0)
            {
                var style = (WindowStyle)NativeMethods.GetWindowLong(hWnd, NativeMethods.GWL_STYLE);

                if ((style & WindowStyle) != WindowStyle)
                    return false;
            }

            if (WindowStyleEx != 0)
            {
                var style = (WindowStyleEx)NativeMethods.GetWindowLong(hWnd, NativeMethods.GWL_EXSTYLE);

                if ((style & WindowStyleEx) != WindowStyleEx)
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Attaches a new window to the filter. The default implementation
        /// parents un-parented windows to the application host. Override to
        /// provide custom logic.
        /// </summary>
        /// <param name="host"></param>
        /// <param name="hWnd"></param>
        public virtual void AttachWindow(AppHost host, IntPtr hWnd)
        {
            // By default we just parent the window to the host. Note that some
            // dialogs may already be parented. These are skipped.

            if (!IsParented(hWnd))
                NativeMethods.SetParent(hWnd, GetHostHandle(host));
        }

        /// <summary>
        /// Detaches a window from the filter. Override to provide custom logic.
        /// </summary>
        /// <param name="host"></param>
        /// <param name="hWnd"></param>
        public virtual void DetachWindow(AppHost host, IntPtr hWnd)
        {
        }

        /// <summary>
        /// Centers the provided window in the application host.
        /// </summary>
        /// <param name="hWnd">Handle to the window to be centered.</param>
        /// <param name="size">Size used to center the window.</param>
        protected void CenterWindow(IntPtr hWnd, Size size)
        {
            var bounds = new NativeMethods.RECT();

            NativeMethods.GetWindowRect(hWnd, ref bounds);

            int width = bounds.right - bounds.left;
            int height = bounds.bottom - bounds.top;

            NativeMethods.MoveWindow(
                hWnd,
                (size.Width - width) / 2,
                (size.Height - height) / 2,
                width,
                height,
                true
            );
        }

        /// <summary>
        /// Maximizes the provided window in the application host.
        /// </summary>
        /// <param name="hWnd">Handle to the window to be maximized.</param>
        /// <param name="size">Size used to maximize the window.</param>
        protected void MaximizeWindow(IntPtr hWnd, Size size)
        {
            NativeMethods.MoveWindow(
                hWnd,
                0,
                0,
                size.Width,
                size.Height,
                true
            );
        }

        private bool IsParented(IntPtr hWnd)
        {
            var parent = NativeMethods.GetParent(hWnd);

            return parent != IntPtr.Zero && parent != _desktopWindow;
        }
    }
}
