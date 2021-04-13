using Microsoft.Win32;
using StoreManagement.Models;
using StoreManagement.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;

namespace StoreManagement.ViewModels
{
    public class ProductViewModel : BaseViewModel
    {
        public ICommand AddWindowProductCommand { get; set; }
        public ICommand CloseWindowCommand { get; set; }
        public ICommand AddProductCommand { get; set; }
        public ICommand SelectImageCommand { get; set; }
        public ICommand SeparateThousandsCommand { get; set; }

        public ProductViewModel()
        {
            AddWindowProductCommand = new RelayCommand<HomeWindow>((para) => true, (para) => OpenAddProductWindow(para));
            CloseWindowCommand = new RelayCommand<AddProductWindow>((para) => true, (para) => CloseWindow(para));
            AddProductCommand = new RelayCommand<AddProductWindow>((para) => true, (para) => AddProduct(para));
            SelectImageCommand = new RelayCommand<Grid>((para) => true, (para) => ChooseImage(para));
            SeparateThousandsCommand = new RelayCommand<TextBox>((para) => true, (para) => SeparateThousands(para));
        }

        private void ChooseImage(Grid para)
        {
            OpenFileDialog op = new OpenFileDialog();
            op.Title = "Chọn ảnh";
            op.Filter = "All supported graphics|*.jpg;*.jpeg;*.png|" + "JPEG (*.jpg;*.jpeg)|*.jpg;*.jpeg|" + "Portable Network Graphic (*.png)|*.png";
            if (op.ShowDialog() == true)
            {
                
            }
        }

        private void AddProduct(AddProductWindow para)
        {
            if (string.IsNullOrEmpty(para.txtName.Text))
            {
                return;
            }
            if (string.IsNullOrEmpty(para.txtUnit.Text))
            {
                return;
            }
            if (string.IsNullOrEmpty(para.txtImportPrice.Text))
            {
                return;
            }
            if (string.IsNullOrEmpty(para.txtExportPrice.Text))
            {
                return;
            }
            if (string.IsNullOrEmpty(para.txtAmount.Text))
            {
                return;
            }

            string id = para.txtID.Text;
            string name = para.txtName.Text;
            string unit = para.txtUnit.Text;
            long importPrice = ConvertToNumber(para.txtImportPrice.Text);
            long exportPrice = ConvertToNumber(para.txtExportPrice.Text);
            int amount = (int)ConvertToNumber(para.txtAmount.Text);

            try
            {
                GOODS product = new GOODS();
                product.ID = int.Parse(id);
                product.NAME = name;
                product.UNIT = unit;
                product.COSTPRICE = importPrice;
                product.PRICE = exportPrice;
                product.COUNT = amount;

                DataProvider.Instance.DB.GOODS.Add(product);
                DataProvider.Instance.DB.SaveChanges();
            }
            catch
            {

            }
            finally
            {
                para.Close();
            }
        }

        private void CloseWindow(AddProductWindow para)
        {
            para.Close();
        }

        void OpenAddProductWindow(Views.HomeWindow para)
        {
            AddProductWindow addProductWindow = new AddProductWindow();

            try
            {
                string query = "SELECT * FROM GOODS";
            
                GOODS product = DataProvider.Instance.DB.GOODS.SqlQuery(query).Last();
                addProductWindow.txtID.Text = (product.ID + 1).ToString();
            }
            catch
            {
                addProductWindow.txtID.Text = "1";
            }
            finally
            {
                addProductWindow.ShowDialog();
            }
        }
    }
}
