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
    public partial class Konsole : Form
    {

        private phim parent;

        public Konsole(phim parent)
        {
            this.parent = parent;
            InitializeComponent();
        }

        private void Konsole_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            textBox1.Lines = parent.console.ToArray();
        }
    }
}
