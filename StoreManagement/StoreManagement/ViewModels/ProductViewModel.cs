using StoreManagement.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace StoreManagement.ViewModels
{
    public class ProductViewModel
    {
        public ICommand AddProductCommand { get; set; }

        public ProductViewModel()
        {
            AddProductCommand = new RelayCommand<Views.HomeWindow>((para) => true, (para) => OpenAddProductWindow(para));
        }

        void OpenAddProductWindow(Views.HomeWindow para)
        {
            AddProductWindow addProductWindow = new AddProductWindow();
            addProductWindow.ShowDialog();
        }
    }
}
