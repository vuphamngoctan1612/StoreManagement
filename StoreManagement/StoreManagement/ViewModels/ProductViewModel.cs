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
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace StoreManagement.ViewModels
{
    public class ProductViewModel : BaseViewModel
    {
        private string imageFileName;

        public HomeWindow HomeWindow { get; set; }
        public ICommand AddWindowProductCommand { get; set; }
        public ICommand CloseWindowCommand { get; set; }
        public ICommand SaveCommand { get; set; }
        public ICommand SelectImageCommand { get; set; }
        public ICommand SeparateThousandsCommand { get; set; }
        public ICommand EditProductCommand { get; set; }
        public ICommand DeleteProductCommand { get; set; }
        public ICommand LoadProductOnWindowCommand { get; set; }
        public ICommand SearchProductCommand { get; set; }

        public ProductViewModel()
        {
            AddWindowProductCommand = new RelayCommand<HomeWindow>((para) => true, (para) => OpenAddProductWindow(para));
            CloseWindowCommand = new RelayCommand<AddProductWindow>((para) => true, (para) => CloseWindow(para));
            SaveCommand = new RelayCommand<AddProductWindow>((para) => true, (para) => AddProduct(para));
            SelectImageCommand = new RelayCommand<Grid>((para) => true, (para) => ChooseImage(para));
            SeparateThousandsCommand = new RelayCommand<TextBox>((para) => true, (para) => SeparateThousands(para));
            EditProductCommand = new RelayCommand<ProductControlUC>((para) => true, (para) => EditProduct(para));
            DeleteProductCommand = new RelayCommand<ProductControlUC>((para) => true, (para) => DeleteProduct(para));
            LoadProductOnWindowCommand = new RelayCommand<HomeWindow>((para) => true, (para) => LoadProduct(para));
            SearchProductCommand = new RelayCommand<HomeWindow>((para) => true, (para) => SearchProduct(para));
        }

        private void SearchProduct(HomeWindow para)
        {
            this.HomeWindow = para;

            foreach (ProductControlUC control in HomeWindow.stkProducts.Children)
            {
                if (!control.txbName.Text.ToLower().Contains(this.HomeWindow.txtSeachProduct.Text))
                {
                    control.Visibility = Visibility.Collapsed;
                }
                else
                {
                    control.Visibility = Visibility.Visible;
                }
            }
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
                if (product.IsDelete == false)
                {
                    ProductControlUC control = new ProductControlUC();
                    control.txbID.Text = product.ID.ToString();
                    control.txbName.Text = product.Name;
                    control.txbUnit.Text = product.Unit;
                    control.txbImportPrice.Text = ConvertToString(product.ImportPrice);
                    control.txbPrice.Text = ConvertToString(product.ExportPrice);
                    control.txbCount.Text = ConvertToString(product.Count);

                    this.HomeWindow.stkProducts.Children.Add(control);
                }
            }
        }

        private void DeleteProduct(ProductControlUC para)
        {
            Product product = new Product();
            int id = int.Parse(para.txbID.Text);
            product = (Product)DataProvider.Instance.DB.Products.Where(x => x.ID == id).First();

            product.IsDelete = true;
            DataProvider.Instance.DB.Products.AddOrUpdate(product);
            DataProvider.Instance.DB.SaveChanges();

            this.HomeWindow.stkProducts.Children.Remove(para);
        }

        private void EditProduct(ProductControlUC para)
        {
            Product product = new Product();
            int id = int.Parse(para.txbID.Text);
            product = (Product)DataProvider.Instance.DB.Products.Where(x => x.ID == id).First();

            ImageBrush imageBrush = new ImageBrush();
            imageBrush.ImageSource = Converter.Instance.ConvertByteToBitmapImage(product.Image);

            AddProductWindow window = new AddProductWindow();

            window.txtID.Text = product.ID.ToString();

            window.txtName.Text = product.Name;
            window.txtName.SelectionStart = window.txtName.Text.Length;

            window.txtUnit.Text = product.Unit;
            window.txtUnit.SelectionStart = window.txtUnit.Text.Length;

            window.txtImportPrice.Text = ConvertToString(product.ImportPrice);
            window.txtImportPrice.SelectionStart = window.txtImportPrice.Text.Length;

            window.txtExportPrice.Text = ConvertToString(product.ExportPrice);
            window.txtExportPrice.SelectionStart = window.txtExportPrice.Text.Length;

            window.txtAmount.Text = ConvertToString(product.Count);
            window.txtAmount.SelectionStart = window.txtAmount.Text.Length;

            window.Title = "Update info product";
            window.grdImage.Background = imageBrush;
            if (window.grdImage.Children.Count > 1)
            {
                window.grdImage.Children.Remove(window.grdImage.Children[0]);
                window.grdImage.Children.Remove(window.grdImage.Children[1]);
            }

            window.ShowDialog();
        }

        private void ChooseImage(Grid para)
        {
            OpenFileDialog op = new OpenFileDialog();
            op.Title = "Chọn ảnh";
            op.Filter = "All supported graphics|*.jpg;*.jpeg;*.png|" + "JPEG (*.jpg;*.jpeg)|*.jpg;*.jpeg|" + "Portable Network Graphic (*.png)|*.png";
            if (op.ShowDialog() == true)
            {
                imageFileName = op.FileName;
                ImageBrush imageBrush = new ImageBrush();
                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.UriSource = new Uri(imageFileName);
                bitmap.EndInit();
                imageBrush.ImageSource = bitmap;
                para.Background = imageBrush;
                if (para.Children.Count > 1)
                {
                    para.Children.Remove(para.Children[0]);
                    para.Children.Remove(para.Children[1]);
                }
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

            try
            {
                string id = para.txtID.Text;
                string name = para.txtName.Text;
                string unit = para.txtUnit.Text;
                long importPrice = ConvertToNumber(para.txtImportPrice.Text);
                long exportPrice = ConvertToNumber(para.txtExportPrice.Text);
                int amount = (int)ConvertToNumber(para.txtAmount.Text);
                
                byte[] imgByteArr;
                if (imageFileName == null)
                {
                    imgByteArr = Converter.Instance.ConvertImageToBytes(@"..\..\Resources\Images\default.jpg");
                }
                else
                {
                    imgByteArr = Converter.Instance.ConvertImageToBytes(imageFileName);
                }
                                
                Product product = new Product();
                product.ID = int.Parse(id);
                product.Name = name;
                product.Unit = unit;
                product.ImportPrice = importPrice;
                product.ExportPrice = exportPrice;
                product.Count = amount;
                product.Image = imgByteArr;
                product.IsDelete = false;

                DataProvider.Instance.DB.Products.AddOrUpdate(product);
                DataProvider.Instance.DB.SaveChanges();
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                LoadProduct(this.HomeWindow);
                para.Close();
            }
        }

        private void OpenAddProductWindow(Views.HomeWindow para)
        {
            AddProductWindow window = new AddProductWindow();
            this.imageFileName = null;
            try
            {
                string query = "SELECT * FROM Product";

                Product product = DataProvider.Instance.DB.Products.SqlQuery(query).Last();
                window.txtID.Text = (product.ID + 1).ToString();
            }
            catch
            {
                window.txtID.Text = "1";
            }
            finally
            {
                window.ShowDialog();
            }
        }

        private void CloseWindow(AddProductWindow para)
        {
            para.Close();
        }
    }
}
