using StoreManagement.Models;
using StoreManagement.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace StoreManagement.ViewModels
{
    public class DistrictViewModel : BaseViewModel
    {
        public ICommand CloseWindowCommand { get; set; }
        public ICommand SaveCommand { get; set; }

        public DistrictViewModel()
        {
            CloseWindowCommand = new RelayCommand<AddDistrictWindow>((para) => true, (para) => CloseWindow(para));
            SaveCommand = new RelayCommand<AddDistrictWindow>((para) => true, (para) => AddDistrict(para));
        }

        private void AddDistrict(AddDistrictWindow para)
        {
            if (string.IsNullOrEmpty(para.txtName.Text))
            {
                para.txtName.Focus();
                para.txtName.Text = "";
                return;
            }

            try
            {
                if (DataProvider.Instance.DB.Districts.ToList().Count < 20)
                {
                    District district = new District();
                    district.Name = para.txtName.Text;
                    district.NumberAgencyInDistrict = 0;

                    DataProvider.Instance.DB.Districts.Add(district);
                    DataProvider.Instance.DB.SaveChanges();
                    
                    para.isSucceed = true;
                }
                else
                {
                    MessageBox.Show("Exceed the number of districts limit: 20");
                    para.isSucceed = false;
                    return;
                }
            }
            catch
            {
                para.isSucceed = false;
            }
            finally
            {
                para.Close();
            }
        }

        private void CloseWindow(AddDistrictWindow para)
        {
            para.Close();
        }
    }
}
