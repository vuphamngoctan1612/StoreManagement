using Microsoft.Win32;
using StoreManagement.Models;
using StoreManagement.Resources.UserControls;
using StoreManagement.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.Entity.Migrations;
using System.IO;
using System.Linq;
using System.Reflection;
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
        private Unit selectedUnits;
        public Unit SelectedUnits
        {
            get => selectedUnits;
            set
            {
                selectedUnits = value;
                OnPropertyChanged("SelectedItemProductType");
            }
        }
        private ObservableCollection<Unit> itemsSourceUnits = new ObservableCollection<Unit>();
        public ObservableCollection<Unit> ItemsSourceUnits
        {
            get => itemsSourceUnits;
            set
            {
                itemsSourceUnits = value;
                OnPropertyChanged();
            }
        }

        private string imageFileName;

        public HomeWindow HomeWindow { get; set; }

        public ICommand LoadProductOnWindowCommand { get; set; }
        public ICommand SelectedChangedUnitsCommand { get; set; }
        public ICommand SearchProductCommand { get; set; }
        public ICommand SeparateThousandsCommand { get; set; }
        //product control
        public ICommand EditProductCommand { get; set; }
        public ICommand DeleteProductCommand { get; set; }
        //add product window
        public ICommand AddProductWindowCommand { get; set; }
        public ICommand CloseWindowCommand { get; set; }
        public ICommand AddProduct_SaveCommand { get; set; }
        public ICommand SelectImageCommand { get; set; }
        public ICommand OpenAddUnitsWindowCommand { get; set; }

        public ICommand ExportExcelProductCommand { get; set; }
        //public ICommand OpenImportProductWindowCommand { get; set; }

        public ProductViewModel()
        {
            LoadProductOnWindowCommand = new RelayCommand<HomeWindow>((para) => true, (para) => LoadProduct(para));
            SelectedChangedUnitsCommand = new RelayCommand<HomeWindow>((para) => true, (para) => SelectedChangeUnits(para));
            SearchProductCommand = new RelayCommand<HomeWindow>((para) => true, (para) => SearchProduct(para));
            SeparateThousandsCommand = new RelayCommand<TextBox>((para) => true, (para) => SeparateThousands(para));
            //product control
            EditProductCommand = new RelayCommand<ProductControlUC>((para) => true, (para) => EditProduct(para));
            DeleteProductCommand = new RelayCommand<ProductControlUC>((para) => true, (para) => DeleteProduct(para));
            //ImportProductCommand = new RelayCommand<ProductControlUC>((para) => true, (para) => ImportProduct(para));
            //add product window
            AddProductWindowCommand = new RelayCommand<HomeWindow>((para) => true, (para) => OpenAddProductWindow(para));
            CloseWindowCommand = new RelayCommand<AddProductWindow>((para) => true, (para) => CloseWindow(para));
            AddProduct_SaveCommand = new RelayCommand<AddProductWindow>((para) => true, (para) => AddProduct(para));
            OpenAddUnitsWindowCommand = new RelayCommand<ComboBox>((para) => true, (para) => OpenAddUnitsWindow(para));
            SelectImageCommand = new RelayCommand<Grid>((para) => true, (para) => ChooseImage(para));

            ExportExcelProductCommand = new RelayCommand<HomeWindow>((para) => true, (para) => ExportExcelProduct(para));
            //OpenImportProductWindowCommand = new RelayCommand<HomeWindow>((para) => true, (para) => OpenImportProductWindow(para));
        }

       

        #region Command        
        // export excel
        public static DataTable ToDataTable<T>(List<T> items)
        {
            DataTable dataTable = new DataTable(typeof(T).Name);

            //Get all the properties
            PropertyInfo[] Props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (PropertyInfo prop in Props)
            {
                //Setting column names as Property names
                dataTable.Columns.Add(prop.Name);
            }
            foreach (T item in items)
            {
                var values = new object[Props.Length];
                for (int i = 0; i < Props.Length; i++)
                {
                    //inserting property values to datatable rows
                    values[i] = Props[i].GetValue(item, null);
                }
                dataTable.Rows.Add(values);
            }
            //put a breakpoint here and check datatable
            return dataTable;
        }
        private void ExportExcelProduct(HomeWindow para)
        {
            List<Product> list = DataProvider.Instance.DB.Products.Where(x => x.IsDelete == false).ToList();

            if (list.Count == 0)
            {
                CustomMessageBox.Show("List product is empty!", "Notify", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            SaveFileDialog sfd = new SaveFileDialog() { Filter = "Excel Workbook|*.xlsx" };
            if (sfd.ShowDialog() == true)
            {
                object misValue = System.Reflection.Missing.Value;
                Microsoft.Office.Interop.Excel.Application application = new Microsoft.Office.Interop.Excel.Application();
                application.Visible = false;
                Microsoft.Office.Interop.Excel.Workbook workbook = application.Workbooks.Add(Microsoft.Office.Interop.Excel.XlWBATemplate.xlWBATWorksheet);
                Microsoft.Office.Interop.Excel.Worksheet worksheet = (Microsoft.Office.Interop.Excel.Worksheet)workbook.ActiveSheet;
                Microsoft.Office.Interop.Excel.Range cellRange;
                DataTable data = new DataTable();

                //using (var reader = ObjectReader.Create(this.ListStores))
                //{
                //    data.Load(reader);
                //}

                data = ToDataTable<Product>(list);
                data.Columns.Remove("Unit");
                data.Columns.Remove("isdelete");
                data.Columns.Remove("InvoiceInfoes");
                data.Columns.Remove("Image");
                data.Columns.Remove("StockReceiptInfoes");

                worksheet = application.Worksheets.Add(misValue, misValue, misValue, misValue);
                worksheet.Name = "Product";
                for (int i = 0; i < data.Columns.Count; i++)
                {
                    worksheet.Cells[1, i + 1] = data.Columns[i].ColumnName;
                }
                for (int i = 0; i < data.Rows.Count; i++)
                {
                    for (int j = 0; j < data.Columns.Count; j++)
                    {
                        worksheet.Cells[i + 2, j + 1] = data.Rows[i][j].ToString();
                    }
                }
                cellRange = worksheet.Range[worksheet.Cells[1, 1], worksheet.Cells[data.Rows.Count + 1, data.Columns.Count]];
                cellRange.EntireColumn.AutoFit();
                workbook.SaveAs(sfd.FileName);
                workbook.Close();
                application.Quit();
            }
        }
        // add product window
        private void OpenAddProductWindow(HomeWindow para)
        {
            AddProductWindow window = new AddProductWindow();
            this.imageFileName = null;
            try
            {
                this.InitItemsSourceUnits();
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
        private void AddProduct(AddProductWindow para)
        {
            if (string.IsNullOrEmpty(para.txtName.Text))
            {
                para.txtName.Focus();
                para.txtName.Text = "";
                return;
            }
            if (string.IsNullOrEmpty(para.cbbUnits.Text))
            {
                para.cbbUnits.Focus();
                para.cbbUnits.Text = "";
                return;
            }
            if (string.IsNullOrEmpty(para.txtImportPrice.Text))
            {
                para.txtImportPrice.Focus();
                para.txtImportPrice.Text = "";
                return;
            }
            if (string.IsNullOrEmpty(para.txtPrice.Text))
            {
                para.txtPrice.Focus();
                para.txtPrice.Text = "";
                return;
            }

            try
            {
                int id = int.Parse(para.txtID.Text);
                string name = para.txtName.Text;
                long importPrice = ConvertToNumber(para.txtImportPrice.Text);
                long price = ConvertToNumber(para.txtPrice.Text);
                byte[] imgByteArr;

                Product product;
                //update product
                if (para.Title == "Update info product")
                {
                    if (imageFileName == null)
                    {
                        imgByteArr = DataProvider.Instance.DB.Products.Where(x => x.ID == id).First().Image;
                    }
                    else
                    {
                        imgByteArr = Converter.Instance.ConvertImageToBytes(imageFileName);
                    }

                    product = DataProvider.Instance.DB.Products.Where(x => x.ID == id).First();
                    product.ID = id;
                    product.Name = name;
                    product.ImportPrice = importPrice;
                    product.ExportPrice = price;
                    product.Image = imgByteArr;
                    product.UnitsID = selectedUnits.ID;
                }
                //add product                
                else
                {
                    if (imageFileName == null)
                    {
                        imgByteArr = Converter.Instance.ConvertImageToBytes(@"..\..\Resources\Images\default.jpg");
                    }
                    else
                    {
                        imgByteArr = Converter.Instance.ConvertImageToBytes(imageFileName);
                    }

                    product = new Product();
                    product.ID = id;
                    product.Name = name;
                    product.ImportPrice = importPrice;
                    product.ExportPrice = price;
                    product.Count = 0;
                    product.Image = imgByteArr;
                    product.IsDelete = false;
                    product.UnitsID = selectedUnits.ID;
                }

                DataProvider.Instance.DB.Products.AddOrUpdate(product);
                DataProvider.Instance.DB.SaveChanges();
            }
            catch (Exception ex)
            {
                CustomMessageBox.Show(ex.Message, "Notify", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                LoadProduct(this.HomeWindow);
                para.Close();
            }
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
        private void CloseWindow(AddProductWindow para)
        {
            para.Close();
        }
        // product control
        private void EditProduct(ProductControlUC para)
        {
            Product product = new Product();
            int id = int.Parse(para.txbID.Text);
            product = (Product)DataProvider.Instance.DB.Products.Where(x => x.ID == id).First();

            ImageBrush imageBrush = new ImageBrush();
            imageBrush.ImageSource = Converter.Instance.ConvertByteToBitmapImage(product.Image);

            AddProductWindow window = new AddProductWindow();
            this.InitItemsSourceUnits();
            window.txtID.Text = product.ID.ToString();

            window.txtName.Text = product.Name;
            window.txtName.SelectionStart = window.txtName.Text.Length;

            window.cbbUnits.Text = this.GetUnitsNameByID(product.UnitsID);
            //window.cbbUnits.SelectionStart = window.cbbUnits.Text.Length;

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
            MessageBoxResult res = CustomMessageBox.Show("Are you sure?", "Notify", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (res == MessageBoxResult.Yes)
            {
                Product product = new Product();
                int id = int.Parse(para.txbID.Text);
                product = (Product)DataProvider.Instance.DB.Products.Where(x => x.ID == id).First();

                product.IsDelete = true;
                DataProvider.Instance.DB.Products.AddOrUpdate(product);
                DataProvider.Instance.DB.SaveChanges();

                this.HomeWindow.stkProducts.Children.Remove(para);
            }
        }

        private void OpenAddUnitsWindow(ComboBox para)
        {
            AddUnitsWindow window = new AddUnitsWindow();

            try
            {
                int id = DataProvider.Instance.DB.Units.Max(p => p.ID) + 1;
                window.txtID.Text = id.ToString();
            }
            catch
            {
                window.txtID.Text = "1";
            }
            finally
            {
                window.ShowDialog();
                if (window.isSaveSucceed)
                {
                    this.InitItemsSourceUnits();
                }
            }
        }
        public void LoadProduct(HomeWindow para)
        {
            this.HomeWindow = para;
            List<Product> products = DataProvider.Instance.DB.Products.Select(p => p).Where(p => p.IsDelete == false).ToList();

            LoadListProduct(products);
            InitItemsSourceUnits();
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
        private void SelectedChangeUnits(HomeWindow para)
        {
            this.HomeWindow = para;
            List<Product> products;

            if (this.HomeWindow.cboSelectFast.SelectedIndex == -1)
            {
                return;
            }
            if (this.HomeWindow.cboSelectFast.SelectedIndex == 0)
            {
                products = DataProvider.Instance.DB.Products.Select(p => p).Where(p => p.IsDelete == false).ToList();

                LoadListProduct(products);
            }
            else
            {
                products = DataProvider.Instance.DB.Products.Select(p => p).Where(p => p.UnitsID == selectedUnits.ID).ToList();

                LoadListProduct(products);
            }
        }
        #endregion

        #region Method
        private void LoadListProduct(List<Product> products)
        {
            this.HomeWindow.stkProducts.Children.Clear();

            foreach (Product product in products)
            {
                ProductControlUC control = new ProductControlUC();
                control.txbID.Text = product.ID.ToString();
                control.txbName.Text = product.Name;
                control.txbUnit.Text = this.GetUnitsNameByID(product.UnitsID);
                control.txbImportPrice.Text = ConvertToString(product.ImportPrice);
                control.txbPrice.Text = ConvertToString(product.ExportPrice);
                control.txbCount.Text = ConvertToString(product.Count);

                this.HomeWindow.stkProducts.Children.Add(control);
            }
        }
        private void InitItemsSourceUnits()
        {
            this.ItemsSourceUnits.Clear();

            Unit unit = new Unit();
            unit.Name = "All";
            this.ItemsSourceUnits.Add(unit);

            List<Unit> listUnits = DataProvider.Instance.DB.Units.Select(p => p).ToList();
            foreach (Unit item in listUnits)
            {
                this.ItemsSourceUnits.Add(item);
            }
        }
        private string GetUnitsNameByID(int? id)
        {
            return DataProvider.Instance.DB.Units.Where(p => p.ID == id).Select(p => p.Name).First();
        }
        #endregion
    }
}