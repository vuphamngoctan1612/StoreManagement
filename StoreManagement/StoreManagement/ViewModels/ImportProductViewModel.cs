using StoreManagement.Models;
using StoreManagement.Resources.UserControls;
using StoreManagement.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace StoreManagement.ViewModels
{
    class ImportProductViewModel : BaseViewModel
    {
        public HomeWindow HomeWindow { get; set; }
        public ImportProductWindow Window { get; set; }

        private ObservableCollection<Unit> itemSourceUnits = new ObservableCollection<Unit>();
        public ObservableCollection<Unit> ItemSourceUnits
        {
            get => itemSourceUnits;
            set
            {
                itemSourceUnits = value;
                OnPropertyChanged();
            }
        }
        private Unit selectedUnits;
        public Unit SelectedUnits
        {
            get => selectedUnits;
            set
            {
                selectedUnits = value;
                OnPropertyChanged();
            }
        }


        private long totalPrice = 0;
        public string TotalPrice { get => SeparateThousands(totalPrice.ToString()); set { totalPrice = ConvertToNumber(value); OnPropertyChanged(); } }

        public ICommand LoadListProductCommand { get; set; }
        public ICommand OpenImportProductWindowCommand { get; set; }
        public ICommand ChangeQuantityCommand { get; set; }
        public ICommand SelectedChangedUnitsCommand { get; set; }
        public ICommand DeleteProductCommand { get; set; }
        public ICommand PayBillCommand { get; set; }
        //public ICommand TextChangedSearchCommand { get; set; }
        public ICommand BackCommand { get; set; }

        public ImportProductViewModel()
        {
            LoadListProductCommand = new RelayCommand<ImportProductWindow>((para) => true, (para) => Loadproduct(para));
            PayBillCommand = new RelayCommand<ImportProductWindow>((para) => true, (para) => Payment(para));
            BackCommand = new RelayCommand<ImportProductWindow>(para => true, para => para.Close());
            //TextChangedSearchCommand = new RelayCommand<ImportProductWindow>((para) => true, (para) => SearchbyName(para));
            SelectedChangedUnitsCommand = new RelayCommand<ImportProductWindow>((para) => true, (para) => SelectedChangeUnits(para));
            ChangeQuantityCommand = new RelayCommand<StockReceiptControlUC>((control) => true, (control) => ChangeCount(control));
            DeleteProductCommand = new RelayCommand<StockReceiptControlUC>((control) => true, (control) => DeleteProduct(control));
            OpenImportProductWindowCommand = new RelayCommand<HomeWindow>((para) => true, (para) => OpenImportProductWindow(para));
        }

        private void SelectedChangeUnits(ImportProductWindow para)
        {
            this.Window = para;
            List<Product> products;

            if (this.Window.cboSelectFast.SelectedIndex == -1)
            {
                return;
            }
            if (this.Window.cboSelectFast.SelectedIndex == 0)
            {
                products = DataProvider.Instance.DB.Products.Where(p => p.IsDelete == false).Select(p => p).ToList();

                LoadListProduct(products);
            }
            else
            {
                products = this.GetListProductByUnits(selectedUnits.ID);

                LoadListProduct(products);
            }
        }

        private void OpenImportProductWindow(HomeWindow para)
        {
            this.HomeWindow = para;
            ImportProductWindow window = new ImportProductWindow();
            try
            {
                string query = "SELECT MAX(ID) FROM StockReceipt";

                List<Int32> temp = DataProvider.Instance.DB.Database.SqlQuery<Int32>(query).ToList();

                window.txbStockID.Text = (temp[0] + 1).ToString();
            }
            catch
            {
                window.txbStockID.Text = "1";
            }
            finally
            {
                window.txbDate.Text = DateTime.Today.ToString("dd/MM/yyyy");

                this.InitItemsSourceUnits();
                window.ShowDialog();
            }
        }

        //private void SearchbyName(ImportProductWindow para)
        //{
        //    this.Window = para;

        //    foreach (StockReceiptControlUC control in this.Window.stkImportProducts.Children)
        //    {
        //        if (!control.txbName.Text.ToLower().Contains(this.Window.txtSearch.Text))
        //        {
        //            control.Visibility = Visibility.Collapsed;
        //        }
        //        else
        //        {
        //            control.Visibility = Visibility.Visible;
        //        }
        //    }
        //}

        private void Payment(ImportProductWindow para)
        {
            this.Window = para;
            if (para.stkImportProducts.Children.Count == 0)
            {
                CustomMessageBox.Show("There are currently no products being imported!", "Notify", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            MessageBoxResult res = CustomMessageBox.Show("Are you sure?", "Confirm", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (res == MessageBoxResult.Yes)
            {
                InvoiceWindow invoiceWindow = new InvoiceWindow();
                StockReceipt stockReceipt = new StockReceipt();
                stockReceipt.ID = int.Parse(this.Window.txbStockID.Text);
                stockReceipt.CheckIn = DateTime.Now;
                stockReceipt.Total = ConvertToNumber(para.txbTotalPrice.Text);

                DataProvider.Instance.DB.StockReceipts.Add(stockReceipt);

                invoiceWindow.txbName.Text = "Our company";
                invoiceWindow.txbAddress.Text = "University of Infomation Technology";
                invoiceWindow.txbPhone.Text = "0xxxxxxxxx";
                invoiceWindow.txbIDinvoice.Text = stockReceipt.ID.ToString();
                invoiceWindow.txbDate.Text = DateTime.Now.ToShortDateString();
                invoiceWindow.txbRetainer.Visibility = System.Windows.Visibility.Hidden;
                invoiceWindow.txbRetainerText.Visibility = System.Windows.Visibility.Hidden;
                invoiceWindow.txbRetainerVND.Visibility = System.Windows.Visibility.Hidden;
                invoiceWindow.txbChange.Visibility = System.Windows.Visibility.Hidden;
                invoiceWindow.txbChangeText.Visibility = System.Windows.Visibility.Hidden;
                invoiceWindow.txbChangeVND.Visibility = System.Windows.Visibility.Hidden;
                invoiceWindow.txbTotal.Text = ConvertToString(stockReceipt.Total);
                invoiceWindow.stkListProductChosenInvoice.Children.Add(new BillUC());

                foreach (StockReceiptControlUC item in this.Window.stkImportProducts.Children)
                {
                    int productID = int.Parse(item.txbID.Text);
                    Product product = DataProvider.Instance.DB.Products.Where(p => p.ID == productID).First();
                    product.Count += long.Parse(item.nsCount.Value.ToString());

                    StockReceiptInfo stockReceiptInfo = new StockReceiptInfo();
                    stockReceiptInfo.StockReceiptID = stockReceipt.ID;
                    stockReceiptInfo.ProductID = productID;
                    stockReceiptInfo.Amount = int.Parse(item.nsCount.Value.ToString());
                    stockReceiptInfo.Price = ConvertToNumber(item.txbPrice.Text);

                    DataProvider.Instance.DB.Products.AddOrUpdate(product);
                    DataProvider.Instance.DB.StockReceiptInfoes.Add(stockReceiptInfo);

                    BillUC billUC = new BillUC();
                    billUC.ID.Text = stockReceiptInfo.ProductID.ToString();
                    billUC.UnitName.Text = stockReceiptInfo.Product.Name.ToString();
                    billUC.Unit.Text = stockReceiptInfo.Product.Unit.Name.ToString();
                    billUC.Amount.Text = ConvertToString(stockReceiptInfo.Amount);
                    billUC.Price.Text = ConvertToString(stockReceiptInfo.Price);
                    billUC.Total.Text = ConvertToString(stockReceiptInfo.Amount * stockReceiptInfo.Price);
                    invoiceWindow.stkListProductChosenInvoice.Children.Add(billUC);
                }
                // update count in list product
                ProductViewModel productViewModel = new ProductViewModel();
                productViewModel.LoadProduct(this.HomeWindow);

                invoiceWindow.Show();

                DataProvider.Instance.DB.SaveChanges();
            }
            else
            {
                return;
            }

            para.Close();
        }

        private void DeleteProduct(StockReceiptControlUC control)
        {
            MessageBoxResult res = CustomMessageBox.Show("Are you sure?", "Notify", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (res == MessageBoxResult.Yes)
            {
                this.Window.stkImportProducts.Children.Remove(control);
            }

            UpdateTotal();
        }

        private void ChangeCount(StockReceiptControlUC control)
        {
            control.txbTotal.Text = SeparateThousands((ConvertToNumber(control.txbPrice.Text) * control.nsCount.Value).ToString());

            UpdateTotal();
        }

        private void Loadproduct(ImportProductWindow para)
        {
            this.Window = para;
            this.Window.stkImportProducts.Children.Clear();
            List<Product> products = DataProvider.Instance.DB.Products.Where(p => p.IsDelete == false).Select(p => p).ToList();

            LoadListProduct(products);
        }

        private void UpdateTotal()
        {
            foreach (StockReceiptControlUC item in this.Window.stkImportProducts.Children)
            {
                this.totalPrice += ConvertToNumber(item.txbTotal.Text);
            }
            this.TotalPrice = this.totalPrice.ToString();
            this.totalPrice = 0;
        }

        #region Method
        private void LoadListProduct(List<Product> products)
        {
            this.Window.stkImportProducts.Children.Clear();

            foreach (Product item in products)
            {
                StockReceiptControlUC control = new StockReceiptControlUC();
                control.txbID.Text = item.ID.ToString();
                control.txbName.Text = item.Name;
                control.txbUnits.Text = DataProvider.Instance.DB.Units.Where(p => p.ID == item.UnitsID).Select(p => p.Name).First();
                control.txbPrice.Text = ConvertToString(item.ImportPrice);
                control.txbTotal.Text = ConvertToString(item.ImportPrice * int.Parse(control.nsCount.Text.ToString()));

                this.Window.stkImportProducts.Children.Add(control);
            }

            UpdateTotal();
        }
        private List<Product> GetListProductByUnits(int unitsID)
        {
            List<Product> res = DataProvider.Instance.DB.Products.Select(p => p).Where(p => p.UnitsID == unitsID).ToList();

            return res;
        }
        private void InitItemsSourceUnits()
        {
            this.ItemSourceUnits.Clear();
            Unit unit = new Unit();
            unit.Name = "All";
            this.ItemSourceUnits.Add(unit);

            List<Unit> listUnits = DataProvider.Instance.DB.Units.Select(p => p).ToList();
            foreach (Unit item in listUnits)
            {
                this.ItemSourceUnits.Add(item);
            }
        }
        #endregion
    }
}
