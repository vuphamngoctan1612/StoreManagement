using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace StoreManagement.Views
{
    /// <summary>
    /// Interaction logic for AddStoreWindow.xaml
    /// </summary>
    public partial class AddStoreWindow : Window
    {
        public AddStoreWindow()
        {
            InitializeComponent();

            this.txtName.Text = null;
            this.txtAddress.Text = null;
            this.txtPhone.Text = null;
            this.txtEmail.Text = null;
            this.dpCheckin.SelectedDate = null;
            this.txtDebt.Text = null;
            this.cbbSpecies.Text = null;
            this.cbDistrict.Text = null;
        }
    }
}
