using StoreManagement.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiveCharts;
using System.Windows.Input;
using StoreManagement.Views;
using System.Windows.Controls;

namespace StoreManagement.ViewModels
{
    public class HomeViewModel : BaseViewModel
    {
        private string uid;

        public ICommand SwitchTabCommand { get; set; }
        public ICommand GetUidCommand { get; set; }

        public HomeViewModel()
        {
            GetUidCommand = new RelayCommand<Button>((para) => true, (para) => uid = para.Uid);
            SwitchTabCommand = new RelayCommand<HomeWindow>((para) => true, (para) => SwitchTab(para));
        }

        private void SwitchTab(HomeWindow para)
        {
            int index = int.Parse(uid);

            para.grdBody_Main.Visibility = System.Windows.Visibility.Hidden;
            para.grdBody_Store.Visibility = System.Windows.Visibility.Hidden;
            para.grdBody_Product.Visibility = System.Windows.Visibility.Hidden;
            para.grdBody_Bill.Visibility = System.Windows.Visibility.Hidden;
            para.grdBody_Report.Visibility = System.Windows.Visibility.Hidden;
            para.grdBody_AccountSetting.Visibility = System.Windows.Visibility.Hidden;


            switch (index)
            {
                case 0:
                    para.grdBody_Main.Visibility = System.Windows.Visibility.Visible;
                    break;
                case 1:
                    para.grdBody_Store.Visibility = System.Windows.Visibility.Visible;
                    break;
                case 2:
                    para.grdBody_Product.Visibility = System.Windows.Visibility.Visible;
                    break;
                case 3:
                    para.grdBody_Bill.Visibility = System.Windows.Visibility.Visible;
                    break;
                case 4:
                    para.grdBody_Report.Visibility = System.Windows.Visibility.Visible;
                    break;
                case 5:
                    para.grdBody_AccountSetting.Visibility = System.Windows.Visibility.Visible;
                    break;
                default:
                    break;
            }
        }
    }
}
