using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.ComponentModel;
using System.Threading;
using System.IO;
using ApplicationHost.Win32;

namespace ApplicationHost
{
    /// <summary>
    /// <see cref="UserControl"/> to host .NET applications inside another .NET
    /// application.
    /// </summary>
    public partial class AppHost : UserControl
    {
        private WindowFilter[] _filters;
        private Thread _thread;
        private AppDomain _appDomain;
        private bool _isFrozen;
        private Dictionary<IntPtr, WindowFilter> _windows;
        private string _assemblyFile;
        private int _windowMutationSuspendCount;
        private List<WindowLog> _windowBackLog;

        /// <summary>
        /// Initializes a new instance of the <see cref="AppHost"/> class.
        /// </summary>
        public AppHost()
        {
            WindowFilters = new WindowFilterCollection();

            CapturePopupWindows = true;
            CaptureMainWindows = true;
            MaximizeMainWindows = true;
        }

        /// <summary>
        /// Raises when the hosted application is closed.
        /// </summary>
        public event EventHandler ApplicationClosed;

        /// <summary>
        /// Raises the <see cref="ApplicationClosed"/> event.
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnApplicationClosed(EventArgs e)
        {
            var ev = ApplicationClosed;

            if (ev != null)
                ev(this, e);
        }

        /// <summary>
        /// Collection of custom <see cref="WindowFilter"/>s. This collection
        /// can be populated with custom implementations of the
        /// <see cref="WindowFilter"/> class allowing custom ways in which to
        /// manipulate windows that are created by the hosted application.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public WindowFilterCollection WindowFilters { get; private set; }

        /// <summary>
        /// Specifies that popup windows that are created by the hosted application
        /// should be captured in the application host. An example of a popup window
        /// is a splash screen.
        /// </summary>
        [DefaultValue(true)]
        [Description("Capture popup windows in the application host")]
        public bool CapturePopupWindows { get; set; }

        /// <summary>
        /// Specifies that main windows that are created by the hosted application
        /// should be captured in the application host.
        /// </summary>
        [DefaultValue(true)]
        [Description("Capture main windows in the application host")]
        public bool CaptureMainWindows { get; set; }

        /// <summary>
        /// Specifies that dialogs that are created by the hosted application should
        /// be captured in the application host.
        /// </summary>
        [DefaultValue(false)]
        [Description("Capture dialogs in the application host")]
        public bool CaptureDialogs { get; set; }

        /// <summary>
        /// Specifies that main windows captured in the application host should
        /// be maximized to the size of the application host and should have its
        /// chrome (caption and sizable border) removed.
        /// </summary>
        [DefaultValue(true)]
        [Description("Maximize main windows and remove its chrome")]
        public bool MaximizeMainWindows { get; set; }

        /// <summary>
        /// Start an assembly and run it inside the application host.
        /// </summary>
        /// <param name="assemblyFile">Path to the executable assembly which
        /// should be run inside the application host.</param>
        public void StartApplication(string assemblyFile)
        {
            if (assemblyFile == null)
                throw new ArgumentNullException("assemblyFile");

            VerifyNotFrozen();

            // Initialize our internal state.

            _isFrozen = true;
            _windows = new Dictionary<IntPtr, WindowFilter>();
            _assemblyFile = assemblyFile;
            _filters = BuildWindowFilters();

            // Create the application domain.

            _appDomain = CreateAppDomain(assemblyFile);

            // Start the thread we're going to use to run the application.

            _thread = new Thread(ThreadProc);

            _thread.SetApartmentState(ApartmentState.STA);

            _thread.Start();
        }

        private void ThreadProc()
        {
            using (new WindowPreCallback(this))
            using (new WindowPostCallback(this))
            {
                try
                {
                    _appDomain.ExecuteAssembly(_assemblyFile);
                }
                finally
                {
                    OnApplicationClosed(EventArgs.Empty);

                    // The AppDomain is unloaded on the main UI thread. This
                    // prevents issues because the AppDomain is using
                    // this thread.

                    try
                    {
                        BeginInvoke(new Action(UnloadAppDomain));
                    }
                    catch
                    {
                        // The control may already have been disposed.
                    }
                }
            }
        }

        private void UnloadAppDomain()
        {
            var appDomain = _appDomain;
            _appDomain = null;

            if (appDomain != null)
                AppDomain.Unload(appDomain);
        }

        private WindowFilter[] BuildWindowFilters()
        {
            var filters = new List<WindowFilter>();

            if (CaptureMainWindows)
                filters.Add(new MainWindowFilter(MaximizeMainWindows));

            if (CapturePopupWindows)
                filters.Add(new PopupWindowFilter());

            if (CaptureDialogs)
                filters.Add(new DialogWindowFilter());

            filters.AddRange(WindowFilters);

            return filters.ToArray();
        }

        /// <summary>
        /// Creates the <see cref="AppDomain"/> used to host the application.
        /// Override to customize how the application domain is created.
        /// </summary>
        /// <param name="assemblyFile">Path to the assembly which will
        /// be executed in the application domain.</param>
        /// <returns>The created application domain.</returns>
        protected virtual AppDomain CreateAppDomain(string assemblyFile)
        {
            var setup = AppDomainSetup;

            if (setup.ApplicationBase == null)
                setup.ApplicationBase = Path.GetDirectoryName(Path.GetFullPath(assemblyFile));

            return AppDomain.CreateDomain(
                setup.ApplicationName,
                AppDomain.CurrentDomain.Evidence,
                setup
            );
        }

        /// <summary>
        /// Gets the <see cref="System.AppDomainSetup"/> used to configure the
        /// <see cref="AppDomain"/> used to host the application. Override to
        /// customize the setup information for the application domain.
        /// </summary>
        protected virtual AppDomainSetup AppDomainSetup
        {
            get
            {
                return new AppDomainSetup
                {
                    ApplicationName = "AppHost Application Domain"
                };
            }
        }

        private void VerifyNotFrozen()
        {
            if (_isFrozen)
                throw new InvalidOperationException("Application already is running");
        }

        private void WindowDestroyed(ref CallWndProcReturnMessage msg)
        {
            WindowFilter filter;

            if (_windows.TryGetValue(msg.HWnd, out filter))
            {
                if (filter != null)
                    filter.DetachWindow(this, msg.HWnd);

                RegisterWindow(new WindowLog(false, msg.HWnd, null));
            }
        }

        private void WindowResized(ref CallWndProcReturnMessage msg)
        {
            WindowFilter filter;

            if (!_windows.TryGetValue(msg.HWnd, out filter))
                filter = ProcessNewWindow(msg.HWnd);

            if (filter != null)
                filter.ResizeWindow(this, msg.HWnd);
        }

        private WindowFilter ProcessNewWindow(IntPtr hWnd)
        {
#if DEBUG
            if (NativeMethods.GetParent(hWnd) == IntPtr.Zero)
            {
                Console.WriteLine(
                    "{0:x}: Style [{1}], StyleEx [{2}]",
                    (int)hWnd,
                    (WindowStyle)NativeMethods.GetWindowLong(hWnd, NativeMethods.GWL_STYLE),
                    (WindowStyleEx)NativeMethods.GetWindowLong(hWnd, NativeMethods.GWL_EXSTYLE)
                );
            }
#endif

            for (int i = 0; i < _filters.Length; i++)
            {
                var filter = _filters[i];

                if (filter.IsMatch(hWnd))
                {
                    RegisterWindow(new WindowLog(true, hWnd, filter));

                    filter.AttachWindow(this, hWnd);

                    return filter;
                }
            }

            RegisterWindow(new WindowLog(true, hWnd, null));

            return null;
        }

        private void WindowShown(ref CallWndProcMessage msg)
        {
            if (!_windows.ContainsKey(msg.HWnd))
                ProcessNewWindow(msg.HWnd);
        }

        /// <param name="e">An <see cref="T:System.EventArgs"/> that contains the event data. </param>
        protected override void OnResize(EventArgs e)
        {
            if (_isFrozen)
            {
                SuspendMutateWindows();

                try
                {
                    foreach (var window in _windows)
                    {
                        if (window.Value != null)
                            window.Value.ResizeWindow(this, window.Key);
                    }
                }
                finally
                {
                    ResumeMutateWindows();
                }
            }

            base.OnResize(e);
        }

        /// <summary>
        /// Closes the hosted application by sending a <c>WM_CLOSE</c> message
        /// to all windows which matched a filter.
        /// </summary>
        public void CloseApplication()
        {
            SuspendMutateWindows();

            try
            {
                foreach (var window in _windows)
                {
                    if (window.Value != null)
                        NativeMethods.PostMessage(window.Key, NativeMethods.WM_CLOSE, UIntPtr.Zero, IntPtr.Zero);
                }
            }
            finally
            {
                ResumeMutateWindows();
            }
        }

        private void SuspendMutateWindows()
        {
            _windowMutationSuspendCount++;
        }

        private void ResumeMutateWindows()
        {
            if (
                --_windowMutationSuspendCount == 0 &&
                _windowBackLog != null
            ) {
                foreach (var logItem in _windowBackLog)
                {
                    ApplyWindowLog(logItem);
                }

                _windowBackLog = null;
            }
        }

        private void RegisterWindow(WindowLog logItem)
        {
            if (_windowMutationSuspendCount > 0)
            {
                if (_windowBackLog == null)
                    _windowBackLog = new List<WindowLog>();

                _windowBackLog.Add(logItem);
            }
            else
            {
                ApplyWindowLog(logItem);
            }
        }

        private void ApplyWindowLog(WindowLog logItem)
        {
            if (logItem.Add)
                _windows[logItem.HWnd] = logItem.Filter;
            else
                _windows.Remove(logItem.HWnd);
        }

        private struct WindowLog
        {
            private bool _add;
            private IntPtr _hWnd;
            private WindowFilter _filter;

            public WindowLog(bool add, IntPtr hWnd, WindowFilter filter)
            {
                _add = add;
                _hWnd = hWnd;
                _filter = filter;
            }

            public bool Add { get { return _add; } }
            public IntPtr HWnd { get { return _hWnd; } }
            public WindowFilter Filter { get { return _filter; } }
        }
    }
}
