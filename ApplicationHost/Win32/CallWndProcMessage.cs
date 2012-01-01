using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace ApplicationHost.Win32
{
    /// <summary>
    /// Defines the message parameters passed to a WH_CALLWNDPROC hook procedure, CallWndProc.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct CallWndProcMessage
    {
        private IntPtr _lParam;
        private IntPtr _wParam;
        private int _msg;
        private IntPtr _hWnd;

        /// <summary>
        /// Additional information about the message. The exact meaning depends on the message value.
        /// </summary>
        public IntPtr LParam
        {
            get { return _lParam; }
            set { _lParam = value; }
        }

        /// <summary>
        /// Additional information about the message. The exact meaning depends on the message value.
        /// </summary>
        public IntPtr WParam
        {
            get { return _wParam; }
            set { _wParam = value; }
        }

        /// <summary>
        /// The message.
        /// </summary>
        public int Msg
        {
            get { return _msg; }
            set { _msg = value; }
        }

        /// <summary>
        /// A handle to the window to receive the message.
        /// </summary>
        public IntPtr HWnd
        {
            get { return _hWnd; }
            set { _hWnd = value; }
        }
    }
}
