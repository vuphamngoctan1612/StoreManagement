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
    /// Interaction logic for AddDistrictWindow.xaml
    /// </summary>
    public partial class AddDistrictWindow : Window
    {
        public bool isSucceed { get; set; }

        public AddDistrictWindow()
        {
            InitializeComponent();
            this.txtName.Text = null;

            this.isSucceed = false;
        }
    }
}
