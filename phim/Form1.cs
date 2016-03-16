using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using CefSharp.WinForms;
using CefSharp;

namespace phim
{
    public partial class phim : Form
    {
        private bool realExit = false;
        private readonly ChromiumWebBrowser browser;
        public List<string> console = new List<string>();
        private Einstellungen set;
        public phim()
        {
            InitializeComponent();
            this.browser = new ChromiumWebBrowser("about:blank")
            {
                Dock = DockStyle.Fill,
            };
            toolStripContainer1.ContentPanel.Controls.Add(this.browser);
            this.browser.RegisterJsObject("bridge", new JSBridge(this));
            this.browser.ConsoleMessage += OnBrowserConsoleMessage;
            //console.Add("init");
            this.checkSettings();
            this.setUrl();
            /*if (Properties.Settings.Default.server == "") {
                Properties.Settings.Default.server = "cloud.furtmeier.it";
                Properties.Settings.Default.Save();
            }

            System.Diagnostics.Debug.WriteLine(Properties.Settings.Default.server);*/
            Microsoft.Win32.SystemEvents.SessionEnded += new Microsoft.Win32.SessionEndedEventHandler(OnSystemLogoff);
        }

        private void OnSystemLogoff(object sender, EventArgs e)
        {
            this.realExit = true;
            Application.Exit();
        }

        private void OnBrowserConsoleMessage(object sender, ConsoleMessageEventArgs args)
        {
            string log = string.Format("Line: {0}, Source: {1}, Message: {2}", args.Line, args.Source, args.Message);
            console.Add(log);
        }

        public void checkSettings()
        {
            if (Properties.Settings.Default.server == "")
            {
                toolStripContainer1.Hide();
                //button1.Show();
                showSettings();
            } else
            {
                toolStripContainer1.Show();
               // button1.Hide();
            }

        }

        private void webBrowser1_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
        }

        private void phim_Load(object sender, EventArgs e)
        {
            if (Properties.Settings.Default.position != null)
            {
                this.Location = Properties.Settings.Default.position;
            }

        }
        
        public void setUrl()
        {
            string url = "https://" + Properties.Settings.Default.server + "/ubiquitous/phim/phim.php?cloud=" + Properties.Settings.Default.cloud + "&username=" + Properties.Settings.Default.user + "&password=" + Properties.Settings.Default.password;
            if (!Uri.IsWellFormedUriString(url, UriKind.RelativeOrAbsolute))
                return;

            browser.Load(url);
            
        }

        private void phim_Resize(object sender, EventArgs e)
        {
            if (FormWindowState.Minimized == this.WindowState)
            {
                this.Hide();
            }
        }

        private void mynotifyicon_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            this.WindowState = FormWindowState.Normal;
            this.Show();
        }

        private void phim_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (this.realExit)
            {
                Properties.Settings.Default.position = this.Location;
                Properties.Settings.Default.Save();

                try {
                    this.browser.GetMainFrame().ExecuteJavaScriptAsync("phimChat.goOffline();");
                } catch (System.Exception)
                {

                }
                System.Threading.Thread.Sleep(500);
                return;
            }

            e.Cancel = true;
            this.Hide();
        }

        private void beendenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.realExit = true;
            Application.Exit();
        }

        private void anzeigenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Normal;
            this.Show();
        }

        private void einstellungenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            showSettings();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            showSettings();
        }

        private void showSettings()
        {
            if(this.set == null)
                this.set = new Einstellungen(this);

            this.set.ShowDialog();
        }

        private void toolStripContainer1_Click(object sender, EventArgs e)
        {

        }

        private void consoleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Konsole set = new Konsole(this);
            set.Show();

        }

        public void TrayBlinkStart()
        {
            mynotifyicon.Icon = Properties.Resources.phimY;
        }

        public void TrayBlinkStop()
        {
            mynotifyicon.Icon = Properties.Resources.phim;
        }
    }

    public class JSBridge
    {
        private phim parent;

        public JSBridge(phim parent)
        {
            this.parent = parent;
        }

        public void Minimize()
        {
            this.parent.WindowState = FormWindowState.Minimized;
        }

        public void TrayBlinkStart()
        {
            this.parent.TrayBlinkStart();
        }

        public void TrayBlinkStop()
        {
            this.parent.TrayBlinkStop();
        }
    }
}
