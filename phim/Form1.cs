using System;
using System.Collections.Generic;
using System.Windows.Forms;
using CefSharp.WinForms;
using CefSharp;
using Microsoft.Win32;
using System.Net;
using System.Xml;

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
            this.WindowState = FormWindowState.Minimized;

            this.browser = new ChromiumWebBrowser("about:blank")
            {
                Dock = DockStyle.Fill,
            };
            toolStripContainer1.ContentPanel.Controls.Add(this.browser);

            this.browser.RegisterJsObject("bridge", new JSBridge(this));
            this.browser.ConsoleMessage += OnBrowserConsoleMessage;

            SystemEvents.PowerModeChanged += OnPowerChange;

            this.checkSettings();
            this.setUrl();
        }

        private void OnPowerChange(object s, PowerModeChangedEventArgs e)
        {
            if (e.Mode == PowerModes.Resume)
                this.setUrl();
            
            if (e.Mode == PowerModes.Suspend)
            {
                try
                {
                    this.browser.GetMainFrame().ExecuteJavaScriptAsync("phimChat.goOffline();");
                }
                catch (System.Exception)
                {

                }

            }
        }

        private void OnBrowserConsoleMessage(object sender, ConsoleMessageEventArgs args)
        {
            string log = string.Format("Line: {0}, Source: {1}, Message: {2}", args.Line, args.Source, args.Message);
            console.Add(log);
        }

        public void checkSettings()
        {
            if (Properties.Settings.Default.server == "" && Properties.Settings.Default.authServerUrl == "" && Properties.Settings.Default.authServerToken == "")
            {
                toolStripContainer1.Hide();
                showSettings();
            } else
                toolStripContainer1.Show();
        }

        private authData getAuthData()
        {

            authData ad = new authData();
            ad.server = Properties.Settings.Default.server;
            ad.cloud = Properties.Settings.Default.cloud;
            ad.username = Properties.Settings.Default.user;
            ad.password = Properties.Settings.Default.password;

            //MessageBox.Show(winName);

            if (Properties.Settings.Default.authServerUrl == "" || Properties.Settings.Default.authServerToken == "")
                return ad;

            string winName = System.Security.Principal.WindowsIdentity.GetCurrent().Name;

            try
            {
                ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };

                WebClient wc = new WebClient();
                var xml = wc.DownloadString("https://" + Properties.Settings.Default.authServerUrl + "/ubiquitous/phim/provisioning.php?token=" + WebUtility.UrlEncode(Properties.Settings.Default.authServerToken) + "&user=" + WebUtility.UrlEncode(winName));

                XmlDocument doc = new XmlDocument();
                doc.LoadXml(xml);
                
                ad.server = doc.DocumentElement.SelectSingleNode("/phynx/phim/server").InnerText;
                ad.cloud = doc.DocumentElement.SelectSingleNode("/phynx/phim/cloud").InnerText;
                ad.username = "";
                ad.password = "";
                ad.token = doc.DocumentElement.SelectSingleNode("/phynx/phim/token").InnerText;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            return ad;
        }

        private void phim_Load(object sender, EventArgs e)
        {
            if (Properties.Settings.Default.position == null)
                return;
            
            if(Properties.Settings.Default.position.Y + this.Height < Screen.FromControl(this).Bounds.Height && Properties.Settings.Default.position.X + this.Width < Screen.FromControl(this).Bounds.Width)
                this.Location = Properties.Settings.Default.position;
        }

        public class authData
        {
            public string username { get; set; }
            public string password { get; set; }
            public string token { get; set; }
            public string server { get; set; }
            public string cloud { get; set; }
        }

        public void setUrl()
        {
            authData ad = this.getAuthData();
            
            string url = "https://" + ad.server + "/ubiquitous/phim/phim.php?cloud=" + ad.cloud + (ad.token != null ? "&token=" + ad.token : "&username=" + ad.username + "&password=" + ad.password);
            //MessageBox.Show(url);
            if (!Uri.IsWellFormedUriString(url, UriKind.RelativeOrAbsolute))
                return;

            browser.Load(url);
        }

        
        private void mynotifyicon_Click(object sender, EventArgs e)
        {
            MouseEventArgs me = (MouseEventArgs)e;
            if (me.Button == System.Windows.Forms.MouseButtons.Right)
                return;
            

            if (FormWindowState.Minimized == this.WindowState)
            {
                this.WindowState = FormWindowState.Normal;
                //MessageBox.Show("Show!");
            }
            else {
                this.WindowState = FormWindowState.Minimized;
                //MessageBox.Show("Hide!");
            }
        }

        private void phim_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.WindowsShutDown)
                this.realExit = true;
            

            if (this.realExit)
            {
                if (this.WindowState == FormWindowState.Normal)
                {
                    Properties.Settings.Default.position = this.Location;
                    Properties.Settings.Default.Save();
                }

                try {
                    this.browser.GetMainFrame().ExecuteJavaScriptAsync("phimChat.goOffline();");
                } catch (System.Exception)
                {

                }
                System.Threading.Thread.Sleep(500);
                return;
            }

            e.Cancel = true;

            this.WindowState = FormWindowState.Minimized;
        }

        private void beendenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.realExit = true;
            Application.Exit();
        }

        private void anzeigenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Normal;
            //this.Show();
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

        private void phim_Resize(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                //this.Hide();
                //MessageBox.Show("Hidden!");
            }

            if (this.WindowState == FormWindowState.Normal)
            {
                //this.Show();
                //MessageBox.Show("Shown!");
            }

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
