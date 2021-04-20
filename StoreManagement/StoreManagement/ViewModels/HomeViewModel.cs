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
using System.Windows.Media;

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
            SolidColorBrush transparent = new SolidColorBrush();
            transparent.Color = Color.FromArgb(0, 0, 0, 0);

            int index = int.Parse(uid);

            para.grdBody_Main.Visibility = System.Windows.Visibility.Hidden;
            para.grdBody_Store.Visibility = System.Windows.Visibility.Hidden;
            para.grdBody_Product.Visibility = System.Windows.Visibility.Hidden;
            para.grdBody_Bill.Visibility = System.Windows.Visibility.Hidden;
            para.grdBody_Report.Visibility = System.Windows.Visibility.Hidden;
            para.grdBody_AccountSetting.Visibility = System.Windows.Visibility.Hidden;

            para.rec_btn_Menu_Main.Fill = transparent;
            para.rec_btn_Menu_Store.Fill = transparent;
            para.rec_btn_Menu_Product.Fill = transparent;
            para.rec_btn_Menu_Bill.Fill = transparent;
            para.rec_btn_Menu_Report.Fill = transparent;
            para.rec_btn_Menu_Setting.Fill = transparent;

            switch (index)
            {
                case 0:
                    para.grdBody_Main.Visibility = System.Windows.Visibility.Visible;
                    para.rec_btn_Menu_Main.Fill = (Brush)new BrushConverter().ConvertFrom("#FF9E9F");
                    break;
                case 1:
                    para.grdBody_Store.Visibility = System.Windows.Visibility.Visible;
                    para.rec_btn_Menu_Store.Fill = (Brush)new BrushConverter().ConvertFrom("#FF9E9F");
                    break;
                case 2:
                    para.grdBody_Product.Visibility = System.Windows.Visibility.Visible;
                    para.rec_btn_Menu_Product.Fill = (Brush)new BrushConverter().ConvertFrom("#FF9E9F");
                    break;
                case 3:
                    para.grdBody_Bill.Visibility = System.Windows.Visibility.Visible;
                    para.rec_btn_Menu_Bill.Fill = (Brush)new BrushConverter().ConvertFrom("#FF9E9F");
                    break;
                case 4:
                    para.grdBody_Report.Visibility = System.Windows.Visibility.Visible;
                    para.rec_btn_Menu_Report.Fill = (Brush)new BrushConverter().ConvertFrom("#FF9E9F");
                    break;
                case 5:
                    para.grdBody_AccountSetting.Visibility = System.Windows.Visibility.Visible;
                    para.rec_btn_Menu_Setting.Fill = (Brush)new BrushConverter().ConvertFrom("#FF9E9F");
                    break;
                default:
                    break;
            }
        }
    }
}