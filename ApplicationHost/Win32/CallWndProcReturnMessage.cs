using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace ApplicationHost.Win32
{
    /// <summary>
    /// Defines the message parameters passed to a WH_CALLWNDPROCRET hook procedure, CallWndRetProc.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct CallWndProcReturnMessage
    {
        private IntPtr _lResult;
        private IntPtr _lParam;
        private IntPtr _wParam;
        private int _msg;
        private IntPtr _hWnd;

        /// <summary>
        /// The return value of the window procedure that processed the message specified by the message value.
        /// </summary>
        public IntPtr LResult
        {
            get { return _lResult; }
            set { _lResult = value; }
        }

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
        /// A handle to the window that processed the message specified by the message value.
        /// </summary>
        public IntPtr HWnd
        {
            get { return _hWnd; }
            set { _hWnd = value; }
        }
    }
}
