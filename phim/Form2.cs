using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace phim
{
    public partial class Einstellungen : Form
    {
        private phim parent;
        public Einstellungen(phim parent)
        {
            this.parent = parent;
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.server = textBox1.Text;
            Properties.Settings.Default.user = textBox2.Text;
            Properties.Settings.Default.password = textBox3.Text;
            Properties.Settings.Default.cloud = textBox4.Text;

            Properties.Settings.Default.Save();
            this.parent.checkSettings();
            this.parent.setUrl();
            this.Hide();
        }

        private void Einstellungen_Load(object sender, EventArgs e)
        {
            textBox1.Text = (Properties.Settings.Default.server == "" ? "cloud.furtmeier.it" : Properties.Settings.Default.server);
            textBox2.Text = Properties.Settings.Default.user;
            textBox3.Text = Properties.Settings.Default.password;
            textBox4.Text = Properties.Settings.Default.cloud;
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
