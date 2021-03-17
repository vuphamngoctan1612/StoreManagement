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

namespace StoreManagement
{
    public partial class fMain : Form
    {
        private GunaButton currentButton;
        private Form activeForm;

        public fMain()
        {
            InitializeComponent();

            foreach (GunaButton previousBtn in flowLayoutPanelBTN.Controls)
            {
                Color color = Color.FromArgb(189, 180, 165);
                previousBtn.ForeColor = color;
                previousBtn.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            }

            fStore frm = new fStore();
            OpenChildForm(frm, flowLayoutPanelBTN.Controls[0]);
        }

        private void ActivateButton(object btnSender)
        {
            if (btnSender != null)
            {
                if (currentButton != (GunaButton)btnSender)
                {
                    DisableButton();
                    currentButton = (GunaButton)btnSender;
                    currentButton.ForeColor = Color.White;
                    currentButton.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                }
            }
        }
        private void DisableButton()
        {
            foreach (GunaButton previousBtn in flowLayoutPanelBTN.Controls)
            {
                if (previousBtn.GetType() == typeof(GunaButton))
                {
                    Color color = Color.FromArgb(189, 180, 165);
                    previousBtn.ForeColor = color;
                    previousBtn.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                }
            }
        }
        private void OpenChildForm(Form childForm, object btnSender)
        {
            ActivateButton(btnSender);
            activeForm = childForm;
            childForm.TopLevel = false;
            childForm.FormBorderStyle = FormBorderStyle.None;
            childForm.Dock = DockStyle.Fill;
            this.pnDesktop.Controls.Add(childForm);
            this.pnDesktop.Tag = childForm;
            childForm.BringToFront();
            childForm.Show();
        }
        private void Reset()
        {
            DisableButton();
            currentButton = null;
        }
        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnMinimize_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            if (txtSearch.Visible == true)
                this.txtSearch.Visible = false;
            else
                this.txtSearch.Visible = true;
        }

        private void btnLogout_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnStore_Click(object sender, EventArgs e)
        {
            fStore frm = new fStore();
            OpenChildForm(frm, sender);
        }

        private void btnGoods_Click(object sender, EventArgs e)
        {
            fGoods frm = new fGoods();
            OpenChildForm(frm, sender);
        }

        private void btnBill_Click(object sender, EventArgs e)
        {
            fBills frm = new fBills();
            OpenChildForm(frm, sender);
        }

        private void btnReport_Click(object sender, EventArgs e)
        {
            fReport frm = new fReport();
            OpenChildForm(frm, sender);
        }
    }
}
