using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Guna.UI.WinForms;
using Guna.UI.Lib;

namespace StoreManagement
{
    public partial class fStore : Form
    {
        public fStore()
        {
            InitializeComponent();

            gunaVScrollBar1.Maximum = flowLayoutPanel1.VerticalScroll.Maximum;
        }

        private void gunaVScrollBar1_Scroll(object sender, ScrollEventArgs e)
        {
            flowLayoutPanel1.VerticalScroll.Value = gunaVScrollBar1.Value;   
        }
    }
}
