﻿using Microsoft.Win32;
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

        public ICommand LoadProductOnWindowCommand { get; set; }
        public ICommand SearchProductCommand { get; set; }

        public ICommand SeparateThousandsCommand { get; set; }
        // product control
        public ICommand EditProductCommand { get; set; }
        public ICommand DeleteProductCommand { get; set; }
        // add product window
        public ICommand AddProductWindowCommand { get; set; }
        public ICommand CloseWindowCommand { get; set; }
        public ICommand AddProduct_SaveCommand { get; set; }
        public ICommand SelectImageCommand { get; set; }
        // import product window
        public ICommand AddImportProductWindowCommand { get; set; }
        public ICommand CloseImportWindowCommand { get; set; }
        public ICommand ImportProductCommand { get; set; }
        public ICommand ImportProduct_SaveCommand { get; set; }

        public ProductViewModel()
        {
            LoadProductOnWindowCommand = new RelayCommand<HomeWindow>((para) => true, (para) => LoadProduct(para));
            SearchProductCommand = new RelayCommand<HomeWindow>((para) => true, (para) => SearchProduct(para));

            SeparateThousandsCommand = new RelayCommand<TextBox>((para) => true, (para) => SeparateThousands(para));

            EditProductCommand = new RelayCommand<ProductControlUC>((para) => true, (para) => EditProduct(para));
            DeleteProductCommand = new RelayCommand<ProductControlUC>((para) => true, (para) => DeleteProduct(para));

            // add product window
            AddProductWindowCommand = new RelayCommand<HomeWindow>((para) => true, (para) => OpenAddProductWindow(para));
            CloseWindowCommand = new RelayCommand<AddProductWindow>((para) => true, (para) => CloseWindow(para));
            AddProduct_SaveCommand = new RelayCommand<AddProductWindow>((para) => true, (para) => AddProduct(para));
            SelectImageCommand = new RelayCommand<Grid>((para) => true, (para) => ChooseImage(para));
            // import product window
            AddImportProductWindowCommand = new RelayCommand<HomeWindow>((para) => true, (para) => OpenImportProductwindow(para));
            CloseImportWindowCommand = new RelayCommand<ImportProductWindow>((para) => true, (para) => CloseImportWindow(para));
            ImportProductCommand = new RelayCommand<ProductControlUC>((para) => true, (para) => ImportProduct(para));   //open import product window to update
            ImportProduct_SaveCommand = new RelayCommand<ImportProductWindow>((para) => true, (para) => Import_SaveProduct(para));
        }

        #region Command
        private void ImportProduct(ProductControlUC para)
        {
            Product product = new Product();
            int productID = int.Parse(para.txbID.Text);
            int stockReceiptID;
            product = DataProvider.Instance.DB.Products.Where(x => x.ID == productID).First();

            try
            {
                stockReceiptID = DataProvider.Instance.DB.StockReceipts.Max(p => p.ID) + 1;
            }
            catch
            {
                stockReceiptID = 1;
            }

            ImageBrush imageBrush = new ImageBrush();
            imageBrush.ImageSource = Converter.Instance.ConvertByteToBitmapImage(product.Image);

            ImportProductWindow window = new ImportProductWindow();

            window.txtID.Text = product.ID.ToString();
            window.txtStockReceiptID.Text = stockReceiptID.ToString();

            window.txtProductName.Text = product.Name;
            window.txtProductName.SelectionStart = window.txtProductName.Text.Length;

            window.txtUnit.Text = product.Unit;
            window.txtUnit.SelectionStart = window.txtUnit.Text.Length;

            window.txtImportPrice.Text = ConvertToString(product.ImportPrice);
            window.txtImportPrice.SelectionStart = window.txtImportPrice.Text.Length;

            window.txtAmount.Text = ConvertToString(product.Count);
            window.txtAmount.SelectionStart = window.txtAmount.Text.Length;

            window.grdImage.Background = imageBrush;
            if (window.grdImage.Children.Count != 0)
            {
                window.grdImage.Children.Remove(window.grdImage.Children[0]);
                window.grdImage.Children.Remove(window.grdImage.Children[0]);
            }

            window.Title = "Import exits product";
            window.ShowDialog();
        }

        private void Import_SaveProduct(ImportProductWindow para)
        {
            if (string.IsNullOrEmpty(para.txtProductName.Text))
            {
                para.txtProductName.Focus();
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
            if (string.IsNullOrEmpty(para.txtAmount.Text))
            {
                para.txtAmount.Focus();
                return;
            }

            try
            {
                int productID = int.Parse(para.txtID.Text);
                int stockReceiptID = int.Parse(para.txtStockReceiptID.Text);
                string productName = para.txtProductName.Text;
                string units = para.txtUnit.Text;
                long importPrice = ConvertToNumber(para.txtImportPrice.Text);
                long amount = ConvertToNumber(para.txtAmount.Text);
                byte[] imgByteArr;

                if (imageFileName == null)
                {
                    imgByteArr = Converter.Instance.ConvertImageToBytes(@"..\..\Resources\Images\default.jpg");
                }
                else
                {
                    imgByteArr = Converter.Instance.ConvertImageToBytes(imageFileName);
                }

                Product product;
                //exits product
                if (para.Title == "Import exits product")
                {
                    product = DataProvider.Instance.DB.Products.Where(p => p.ID == productID).First();
                    product.ID = productID;
                    product.Name = productName;
                    product.Unit = units;
                    product.ImportPrice = importPrice;
                    product.Count = amount;
                    product.Image = imgByteArr;
                    product.IsDelete = false;
                }
                //import new product
                else
                {
                    product = new Product();
                    product.ID = productID;
                    product.Name = productName;
                    product.Unit = units;
                    product.ImportPrice = importPrice;
                    product.ExportPrice = 0;
                    product.Count = amount;
                    product.Image = imgByteArr;
                    product.IsDelete = false;
                }

                StockReceiptInfo stockReceiptInfo = new StockReceiptInfo();
                stockReceiptInfo.StockReceiptID = stockReceiptID;
                stockReceiptInfo.ProductID = productID;
                stockReceiptInfo.Amount = amount;
                stockReceiptInfo.Price = importPrice;

                StockReceipt stockReceipt = new StockReceipt();
                stockReceipt.ID = stockReceiptID;
                stockReceipt.CheckIn = DateTime.Now;
                stockReceipt.Total = product.Count * product.ImportPrice;

                DataProvider.Instance.DB.Products.AddOrUpdate(product);
                DataProvider.Instance.DB.StockReceipts.AddOrUpdate(stockReceipt);
                DataProvider.Instance.DB.StockReceiptInfoes.AddOrUpdate(stockReceiptInfo);
                DataProvider.Instance.DB.SaveChanges();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                this.LoadProduct(this.HomeWindow);
                para.Close();
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
            if (string.IsNullOrEmpty(para.txtPrice.Text))
            {
                para.txtUnit.Focus();
                return;
            }

            try
            {
                int id = int.Parse(para.txtID.Text);
                string name = para.txtName.Text;
                string unit = para.txtUnit.Text;
                long price = ConvertToNumber(para.txtPrice.Text);
                
                byte[] imgByteArr;
                if (imageFileName == null)
                {
                    imgByteArr = Converter.Instance.ConvertImageToBytes(@"..\..\Resources\Images\default.jpg");
                }
                else
                {
                    imgByteArr = Converter.Instance.ConvertImageToBytes(imageFileName);
                }

                Product product;
                //update product
                if (para.Title == "Update info product")    
                {
                    product = DataProvider.Instance.DB.Products.Where(x => x.ID == id).First();
                    product.ID = id;
                    product.Name = name;
                    product.Unit = unit;
                    product.ExportPrice = price;
                    product.Image = imgByteArr;
                }
                //add product                
                else
                {
                    product = new Product();
                    product.ID = id;
                    product.Name = name;
                    product.Unit = unit;
                    product.ImportPrice = 0;
                    product.ExportPrice = price;
                    product.Count = 0;
                    product.Image = imgByteArr;
                    product.IsDelete = false;
                }
                this.AddOrUpdateProduct(product);
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

            window.txtPrice.Text = ConvertToString(product.ExportPrice);
            window.txtPrice.SelectionStart = window.txtPrice.Text.Length;

            window.Title = "Update info product";
            window.grdImage.Background = imageBrush;
            if (window.grdImage.Children.Count != 0)
            {
                window.grdImage.Children.Remove(window.grdImage.Children[0]);
                window.grdImage.Children.Remove(window.grdImage.Children[0]);
            }

            window.ShowDialog();
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
                if (para.Children.Count != 0)
                {
                    para.Children.Remove(para.Children[0]);
                    para.Children.Remove(para.Children[0]);
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

        private void OpenImportProductwindow(HomeWindow para)
        {
            this.HomeWindow = para;
            ImportProductWindow window = new ImportProductWindow();
            this.imageFileName = null;
            try
            {
                string query = "SELECT MAX(ID) FROM Product " +
                            "UNION " +
                            "SELECT MAX(ID) FROM StockReceipt";

                List<Int32> temp = DataProvider.Instance.DB.Database.SqlQuery<Int32>(query).ToList();

                window.txtID.Text = (temp[0] + 1).ToString();
                window.txtStockReceiptID.Text = (temp[1] + 1).ToString();
            }
            catch
            {
                window.txtID.Text = "1";
                window.txtStockReceiptID.Text = "1";
            }
            finally
            {
                window.ShowDialog();
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

        private void CloseImportWindow(ImportProductWindow para)
        {
            para.Close();
        }

        private void CloseWindow(AddProductWindow para)
        {
            para.Close();
        }
        #endregion

        #region Method
        private void AddOrUpdateProduct(Product product)
        {
            DataProvider.Instance.DB.Products.AddOrUpdate(product);
            DataProvider.Instance.DB.SaveChanges();
        }
        #endregion
    }
}
