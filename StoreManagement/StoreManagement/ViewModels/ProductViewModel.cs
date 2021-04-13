using Microsoft.Win32;
using StoreManagement.Models;
using StoreManagement.Resources.UserControls;
using StoreManagement.Views;
using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace StoreManagement.ViewModels
{
    public class ProductViewModel : BaseViewModel
    {
        public HomeWindow HomeWindow { get; set; }
        public ICommand AddWindowProductCommand { get; set; }
        public ICommand CloseWindowCommand { get; set; }
        public ICommand SaveCommand { get; set; }
        public ICommand SelectImageCommand { get; set; }
        public ICommand SeparateThousandsCommand { get; set; }
        public ICommand EditProductCommand { get; set; }
        public ICommand DeleteProductCommand { get; set; }
        public ICommand LoadProductOnWindowCommand { get; set; }


        public ProductViewModel()
        {
            AddWindowProductCommand = new RelayCommand<HomeWindow>((para) => true, (para) => OpenAddProductWindow(para));
            CloseWindowCommand = new RelayCommand<AddProductWindow>((para) => true, (para) => CloseWindow(para));
            SaveCommand = new RelayCommand<AddProductWindow>((para) => true, (para) => AddProduct(para));
            SelectImageCommand = new RelayCommand<Grid>((para) => true, (para) => ChooseImage(para));
            SeparateThousandsCommand = new RelayCommand<TextBox>((para) => true, (para) => SeparateThousands(para));
            EditProductCommand = new RelayCommand<TextBlock>((para) => true, (para) => EditProduct(para));
            DeleteProductCommand = new RelayCommand<TextBlock>((para) => true, (para) => DeleteProduct(para));
            LoadProductOnWindowCommand = new RelayCommand<HomeWindow>((para) => true, (para) => LoadProduct(para));
        }

        private void LoadProduct(HomeWindow para)
        {
            this.HomeWindow = para;
            this.HomeWindow.stkProducts.Children.Clear();
            List<GOODS> products = new List<GOODS>();

            string query = "SELECT * FROM GOODS";
            products = DataProvider.Instance.DB.GOODS.SqlQuery(query).ToList();

            foreach (GOODS product in products)
            {
                ProductControlUC control = new ProductControlUC();
                control.txbID.Text = product.ID.ToString();
                control.txbName.Text = product.NAME;
                control.txbUnit.Text = product.UNIT;
                control.txbPrice.Text = string.Format("{0:N0}", product.PRICE);
                control.txbCount.Text = string.Format("{0:N0}", product.COUNT);

                this.HomeWindow.stkProducts.Children.Add(control);
            }
        }

        private void DeleteProduct(TextBlock para)
        {
            
        }

        private void EditProduct(TextBlock para)
        {
            GOODS product = new GOODS();
            int id = int.Parse(para.Text);
            product = (GOODS)DataProvider.Instance.DB.GOODS.Where(x => x.ID == id).First();

            AddProductWindow window = new AddProductWindow();
            window.txtID.Text = product.ID.ToString();
            window.txtName.Text = product.NAME;
            window.txtUnit.Text = product.UNIT;
            window.txtImportPrice.Text = string.Format("{0:N0}", product.COSTPRICE);
            window.txtExportPrice.Text = string.Format("{0:N0}", product.PRICE);
            window.txtAmount.Text = string.Format("{0:N0}", product.COUNT);
            window.Title = "Update info product";
            window.ShowDialog();
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

                DataProvider.Instance.DB.GOODS.AddOrUpdate(product);
                DataProvider.Instance.DB.SaveChanges();
            }
            catch (Exception ex)
            {
                MessageBox.Show("exception: " + ex);
            }
            finally
            {
                LoadProduct(this.HomeWindow);
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
