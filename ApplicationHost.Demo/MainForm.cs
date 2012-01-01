using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace ApplicationHost.Demo
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Shown(object sender, EventArgs e)
        {
            // Place an application in a location where it can be found by
            // the demo host and uncomment the line below.

            // StartApplication("Applications\\LINQPad\\LINQPad.exe");
        }

        private void StartApplication(string assemblyFile)
        {
            var tabPage = new TabPage
            {
                Text = Path.GetFileName(assemblyFile)
            };

            var appHost = new AppHost
            {
                Dock = DockStyle.Fill,
                CaptureDialogs = true
            };

            tabPage.Controls.Add(appHost);

            _tabControl.TabPages.Add(tabPage);

            appHost.StartApplication(assemblyFile);
            appHost.ApplicationClosed += (s, e) => CloseTab(tabPage);
        }

        private void CloseTab(TabPage tabPage)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<TabPage>(CloseTab), tabPage);
            }
            else
            {
                _tabControl.TabPages.Remove(tabPage);

                if (_tabControl.TabPages.Count == 0)
                    Close();
            }
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                foreach (TabPage tabPage in _tabControl.TabPages)
                {
                    var appHost = (AppHost)tabPage.Controls.Cast<AppHost>().Single();

                    appHost.CloseApplication();

                    e.Cancel = true;
                }
            }
        }
    }
}
