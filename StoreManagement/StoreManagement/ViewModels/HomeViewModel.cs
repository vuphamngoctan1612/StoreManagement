using StoreManagement.Models;
using StoreManagement.Resources.UserControls;
using StoreManagement.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows;
using System.Windows.Controls;

namespace StoreManagement.ViewModels
{
    public class HomeViewModel
    {
        public ICommand SwitchTabCommand { get; set; }
        public ICommand GetUidCommand { get; set; }
        private string uid;
        public HomeViewModel()
        {
            SwitchTabCommand = new RelayCommand<Home>((paramater) => true, (paramater) => SwitchTab(paramater));
            GetUidCommand = new RelayCommand<Button>((parameter) => true, (parameter) => uid = parameter.Uid);

        }
        public void SwitchTab(Home paramater)
        {
            int index = int.Parse(uid);

            BillHomeUC billHomeUC = new BillHomeUC();            
            AccountSettingUC acountSettingUC = new AccountSettingUC();
            ReportUC reportUC = new ReportUC();
            paramater.switchGrid.Children.Add(billHomeUC);
            paramater.switchGrid.Children.Add(acountSettingUC);
            paramater.switchGrid.Children.Add(reportUC);
            billHomeUC.Visibility = Visibility.Collapsed;
            acountSettingUC.Visibility = Visibility.Collapsed;
            reportUC.Visibility = Visibility.Collapsed;
            paramater.switchGrid.Visibility = Visibility.Visible;
            switch (index)
            {

                case 2:
                    BillViewModel billViewModel = new BillViewModel();
                    billHomeUC.Visibility = Visibility.Visible;
                    acountSettingUC.Visibility = Visibility.Collapsed;
                    reportUC.Visibility = Visibility.Collapsed;
                    break;
                case 3:
                    billHomeUC.Visibility = Visibility.Collapsed;
                    acountSettingUC.Visibility = Visibility.Collapsed;
                    reportUC.Visibility = Visibility.Visible;
                    break;
                case 4:
                    billHomeUC.Visibility = Visibility.Collapsed;
                    acountSettingUC.Visibility = Visibility.Visible;
                    reportUC.Visibility = Visibility.Collapsed;
                    break;

            }    
        }
    }
}
