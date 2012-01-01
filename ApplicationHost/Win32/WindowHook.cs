using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

// We need to call AppDomain.GetCurrentThreadId.
#pragma warning disable 618

namespace ApplicationHost.Win32
{
    internal abstract class WindowHook : IDisposable
    {
        private bool _disposed;
        private IntPtr _handle;
        private WindowHookProc _hookProc; // Keep a reference to prevent garbage collection.

        public WindowHook(WindowHookType type)
        {
            _hookProc = new WindowHookProc(HookProc);

            _handle = SetWindowsHookEx(
                (int)type,
                _hookProc,
                IntPtr.Zero,
                AppDomain.GetCurrentThreadId()
            );
        }

        ~WindowHook()
        {
            Dispose(false);
        }

        protected virtual IntPtr HookProc(int code, IntPtr wParam, IntPtr lParam)
        {
            return CallNextHookEx(_handle, code, wParam, lParam);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (_handle != IntPtr.Zero)
                {
                    var handle = _handle;
                    _handle = IntPtr.Zero;

                    UnhookWindowsHookEx(handle);
                }

                _disposed = true;
            }
        }

        [DllImport("user32.dll")]
        private static extern IntPtr SetWindowsHookEx(int hookId, WindowHookProc callback, IntPtr hInstance, int nativeThreadId);
        [DllImport("user32.dll")]
        private static extern int UnhookWindowsHookEx(IntPtr idHook);
        [DllImport("user32.dll")]
        private static extern IntPtr CallNextHookEx(IntPtr idHook, int nCode, IntPtr wParam, IntPtr lParam);

        private const int WH_CALLWNDPROCRET = 12;

        private delegate IntPtr WindowHookProc(int code, IntPtr wParam, IntPtr lParam);
    }

    internal abstract class WindowHook<T> : WindowHook
    {
        protected WindowHook(WindowHookType type)
            : base(type)
        {
        }

        protected sealed override IntPtr HookProc(int code, IntPtr wParam, IntPtr lParam)
        {
            if (code >= 0)
            {
                var msg = (T)Marshal.PtrToStructure(lParam, typeof(T));

                Callback(ref msg);
            }

            return base.HookProc(code, wParam, lParam);
        }

        protected abstract void Callback(ref T msg);
    }
}
