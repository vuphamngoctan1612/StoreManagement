using StoreManagement.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiveCharts;
using System.Windows.Input;
using StoreManagement.Views;

namespace StoreManagement.ViewModels
{
    public class HomeViewModel : BaseViewModel
    {
        public ICommand Formatter { get; set; }
        public ICommand OpenProductsWindowCommand { get; set; }
        public ICommand AddProductCommand { get; set; }

        public HomeViewModel()
        {
            OpenProductsWindowCommand = new RelayCommand<Views.HomeWindow>((para) => true, (para) => OpenProductsWindow(para));
        }

        void OpenProductsWindow(Views.HomeWindow para)
        {
            para.grdMain.Visibility = System.Windows.Visibility.Hidden;
            para.grdProduct.Visibility = System.Windows.Visibility.Visible;
        }
    }
}
