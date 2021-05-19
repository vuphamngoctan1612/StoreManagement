using Microsoft.Win32;
using StoreManagement.Models;
using StoreManagement.Resources.UserControls;
using StoreManagement.Views;
using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.IO;
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
            List<Product> products = new List<Product>();

            string query = "SELECT * FROM Product";
            products = DataProvider.Instance.DB.Products.SqlQuery(query).ToList();

            foreach (Product product in products)
            {
                ProductControlUC control = new ProductControlUC();
                control.txbID.Text = product.ID.ToString();
                control.txbName.Text = product.Name;
                control.txbUnit.Text = product.Unit;
                control.txbImportPrice.Text = string.Format("{0:N0}", product.ImportPrice);
                control.txbPrice.Text = string.Format("{0:N0}", product.ExportPrice);
                control.txbCount.Text = string.Format("{0:N0}", product.Count);

                this.HomeWindow.stkProducts.Children.Add(control);
            }
        }

        private void DeleteProduct(TextBlock para)
        {
            
        }

        private void EditProduct(TextBlock para)
        {
            Product product = new Product();
            int id = int.Parse(para.Text);
            product = (Product)DataProvider.Instance.DB.Products.Where(x => x.ID == id).First();

            AddProductWindow window = new AddProductWindow();
            window.txtID.Text = product.ID.ToString();
            window.txtName.Text = product.Name;
            window.txtUnit.Text = product.Unit;
            window.txtImportPrice.Text = string.Format("{0:N0}", product.ImportPrice);
            window.txtExportPrice.Text = string.Format("{0:N0}", product.ExportPrice);
            window.txtAmount.Text = string.Format("{0:N0}", product.Count);
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
                para.txtName.Focus();
                return;
            }
            if (string.IsNullOrEmpty(para.txtUnit.Text))
            {
                para.txtUnit.Focus();
                return;
            }
            if (string.IsNullOrEmpty(para.txtImportPrice.Text))
            {
                para.txtImportPrice.Focus();
                return;
            }
            if (string.IsNullOrEmpty(para.txtExportPrice.Text))
            {
                para.txtExportPrice.Focus();
                return;
            }
            if (string.IsNullOrEmpty(para.txtAmount.Text))
            {
                para.txtAmount.Focus();
                return;
            }

            StreamReader sr = new StreamReader("../../cache.txt");
            string cache = sr.ReadToEnd();
            sr.Close();

            string[] rulesSetting = cache.Split(' ');

            var results = DataProvider.Instance.DB.Products.Where(x => x.IsDelete == false).Select(x => x.Unit).Distinct().ToList();

            if (para.Title == "Thêm sản phẩm" && DataProvider.Instance.DB.Products.Where(x => x.IsDelete == false).ToList().Count >= int.Parse(rulesSetting[2]))
            {
                MessageBox.Show("Exceed the number of product limit");
                return;
            }

            if (DataProvider.Instance.DB.Products.Where(x => x.IsDelete == false).Where(x => x.Unit == para.txtUnit.Text).ToList().Count < 1)
            {
                if (results.Count >= int.Parse(rulesSetting[3]))
                {
                    MessageBox.Show("Exceed the number of unit limit");
                    return;
                }
            }

            string id = para.txtID.Text;
            string name = para.txtName.Text;
            string unit = para.txtUnit.Text;
            long importPrice = ConvertToNumber(para.txtImportPrice.Text);
            long exportPrice = ConvertToNumber(para.txtExportPrice.Text);
            int amount = (int)ConvertToNumber(para.txtAmount.Text);

            try
            {
                Product product = new Product();
                product.ID = int.Parse(id);
                product.Name = name;
                product.Unit = unit;
                product.ImportPrice = importPrice;
                product.ExportPrice = exportPrice;
                product.Count= amount;

                DataProvider.Instance.DB.Products.AddOrUpdate(product);
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
                string query = "SELECT * FROM Product";

                Product product = DataProvider.Instance.DB.Products.SqlQuery(query).Last();
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
