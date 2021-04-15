using StoreManagement.Models;
using StoreManagement.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace StoreManagement.ViewModels
{
    public class ReportViewModel : BaseViewModel
    {
        public ICommand LoadTop4AgencyCommand { get; set; }

        public ReportViewModel()
        {
            LoadTop4AgencyCommand = new RelayCommand<HomeWindow>((para) => true, (para) => LoadTop4Agency(para));
        }

        private void LoadTop4Agency(HomeWindow para)
        {

        }
    }
}
