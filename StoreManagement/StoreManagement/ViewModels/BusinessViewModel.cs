using StoreManagement.Models;
using StoreManagement.Resources.UserControls;
using StoreManagement.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Configuration;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace StoreManagement.ViewModels
{
    class BusinessViewModel : BaseViewModel
    {
        public HomeWindow HomeWindow { get; set; }

        public List<Product> ListProduct;
        public List<Product> ListProductChosen;
        public List<Agency> ListAgency { get; set; }
        public ICommand LoadBusinessWindowCommand { get; set; }
        public ICommand ChosenProductCommand { get; set; }
        public ICommand RemovefromListChosenCommand { get; set; }
        public ICommand CalculationPaymentCommand { get; set; }
        public ICommand AddAgencytoPaymentCommand { get; set; }
        public ICommand PayBusinessCommand { get; set; }
        public ICommand SearchProductBusinessCommand { get; set; }
        public ICommand PrintInvoiceCommand { get; set; }
        public ICommand ReloadBusinessTagCommand { get; set; }

        public BusinessViewModel()
        {
            string query = "SELECT * FROM AGENCY WHERE ISDELETE = 0";
            this.ListAgency = DataProvider.Instance.DB.Agencies.SqlQuery(query).ToList<Agency>();
            query = "SELECT * FROM PRODUCT WHERE ISDELETE = 0";
            this.ListProduct = DataProvider.Instance.DB.Products.SqlQuery(query).ToList();
            this.ListProductChosen = new List<Product>();
            LoadBusinessWindowCommand = new RelayCommand<HomeWindow>((para) => true, (para) => LoadBusiness(para));
            ChosenProductCommand = new RelayCommand<BusinessProductUC>((para) => true, (para) => LoadListChosen(para));
            RemovefromListChosenCommand = new RelayCommand<BusinessProductChosenUC>((para) => true, (para) => RemovefromListChosen(para));
            CalculationPaymentCommand = new RelayCommand<TextBox>((para) => true, (para) => {
                SeparateThousands(para);
                LoadPayment(para);
            });
            AddAgencytoPaymentCommand = new RelayCommand<ComboBox>((para) => true, (para) => AddAgencytoPayment(para));
            PayBusinessCommand = new RelayCommand<HomeWindow>((para) => true, (para) => PayBusiness(this.HomeWindow));
            SearchProductBusinessCommand = new RelayCommand<HomeWindow>((para) => true, (para) => SearchProductBusiness(para));
            PrintInvoiceCommand = new RelayCommand<InvoiceWindow>((para) => true, (para) => PrintInvoice(para));
            ReloadBusinessTagCommand = new RelayCommand<HomeWindow>((para) => true, (para) => ReloadBusiness());
        }

        private void PrintInvoice(InvoiceWindow para)
        {
            try
            {
                para.btnPrint.IsEnabled = false;

                PrintDialog printDialog = new PrintDialog();
                if (printDialog.ShowDialog() == true)
                {
                    printDialog.PrintVisual(para, "Invoice");
                }
            }
            finally
            {
                para.btnPrint.IsEnabled = true;
            }
        }

        private void SearchProductBusiness(HomeWindow para)
        {
            this.HomeWindow = para;
            foreach (BusinessProductUC control in HomeWindow.stkListProductBusiness.Children)
            {
                if (!control.txbNameProductBusinessUC.Text.ToLower().Contains(this.HomeWindow.txbSearchProductsBusiness.Text))
                {
                    control.Visibility = Visibility.Collapsed;
                }
                else
                {
                    control.Visibility = Visibility.Visible;
                }
            }
        }

        private void PayBusiness(HomeWindow para)
        {
            if (para.txbIDAgencyPayment.Text == "-1")
            {
                MessageBox.Show("Vui lòng chọn đại lý");
                return;
            }
            if (ListProductChosen.Count == 0)
            {
                MessageBox.Show("Vui lòng chọn sản phẩm");
                return;
            }

            MessageBoxResult mes = MessageBox.Show("Are you sure?", "Confirm", MessageBoxButton.YesNo);

            if (mes != MessageBoxResult.Yes)
            {
                return;
            }

            InvoiceWindow wdInvoice = new InvoiceWindow();

            wdInvoice.stkListInvoiceInfos.Children.Add(new InvoiceBusinessUC());

            foreach (BusinessProductChosenUC item in this.HomeWindow.stkListProductChosenBusiness.Children)
            {
                InvoiceBusinessUC uc = new InvoiceBusinessUC();

                uc.txbName.Text = item.txbName.Text;
                uc.txbPrice.Text = item.txbPrice.Text;
                uc.txbAmount.Text = item.txbAmount.Text;
                uc.txbUnit.Text = item.txbUnit.Text;
                uc.txbTotal.Text = item.txbTotal.Text;
                uc.txbID.Text = item.txbID.Text;

                wdInvoice.stkListInvoiceInfos.Children.Add(uc);
            }

            wdInvoice.txbTotal.Text = this.HomeWindow.TotalFeeofProductChosenPayment.Text;
            wdInvoice.txbPrepay.Text = this.HomeWindow.txtRetainerPaymment.Text;
            wdInvoice.txbDebt.Text = this.HomeWindow.txbChangePayment.Text;
            wdInvoice.txbName.Text = this.HomeWindow.txbAgencyinPayment.Text;
            wdInvoice.txbPhone.Text = this.HomeWindow.txbPhoneNumberinPayment.Text;
            wdInvoice.txbAddress.Text = this.HomeWindow.txbAddressinPayment.Text;

            try
            {
                string query = "SELECT * FROM Invoice";

                Invoice invoice = DataProvider.Instance.DB.Invoices.SqlQuery(query).Last();
                wdInvoice.txbInvoiceID.Text = (invoice.ID + 1).ToString();
            }
            catch
            {
                wdInvoice.txbInvoiceID.Text = "1";
            }
            wdInvoice.txbInvoiceDate.Text = DateTime.Now.ToShortDateString();

            Invoice inv = new Invoice();
            inv.ID = int.Parse(wdInvoice.txbInvoiceID.Text);
            inv.AgencyID = int.Parse(this.HomeWindow.txbIDAgencyPayment.Text);
            inv.Checkout = DateTime.Parse(wdInvoice.txbInvoiceDate.Text);
            inv.Debt = ConvertToNumber(wdInvoice.txbDebt.Text);
            //inv.Total = ConvertToNumber(wdInvoice.txbTotal.Text);

            DataProvider.Instance.DB.Invoices.Add(inv);

            for (int i = 1; i < wdInvoice.stkListInvoiceInfos.Children.Count; i++)
            {
                InvoiceInfo invInfo = new InvoiceInfo();
                InvoiceBusinessUC item = (InvoiceBusinessUC)(wdInvoice.stkListInvoiceInfos.Children[i]);

                invInfo.InvoiceID = inv.ID;
                invInfo.ProductID = int.Parse(item.txbID.Text);
                invInfo.Amount = int.Parse(item.txbAmount.Text);
                invInfo.Total = ConvertToNumber(item.txbTotal.Text);

                DataProvider.Instance.DB.InvoiceInfoes.Add(invInfo);
            }

            DataProvider.Instance.DB.SaveChanges();
            wdInvoice.ShowDialog();

            ReloadBusiness();
        }

        private void ReloadBusiness()
        {
            string query = "SELECT * FROM AGENCY WHERE ISDELETE = 0";
            this.ListAgency = DataProvider.Instance.DB.Agencies.SqlQuery(query).ToList<Agency>();
            this.HomeWindow.stkListProductChosenBusiness.Children.Clear();
            string query1 = "SELECT * FROM PRODUCT WHERE ISDELETE = 0";
            this.ListProduct = DataProvider.Instance.DB.Products.SqlQuery(query1).ToList();
            this.HomeWindow.stkListProductBusiness.Children.Clear();
            LoadBusiness(this.HomeWindow);
            this.ListProductChosen.Clear();
            this.HomeWindow.txbIDAgencyPayment.Text = "-1";
            this.HomeWindow.txbAgencyinPayment.Text = "";
            this.HomeWindow.txbAddressinPayment.Text = "";
            this.HomeWindow.txbPhoneNumberinPayment.Text = "";
            this.HomeWindow.TotalFeeofProductChosenPayment.Text = "0";
            this.HomeWindow.txtRetainerPaymment.Text = "0";
            this.HomeWindow.txbChangePayment.Text = "0";
            this.HomeWindow.cbSearchAgency.ItemsSource = this.ListAgency;
            this.HomeWindow.cbSearchAgency.SelectedValuePath = "ID";
            this.HomeWindow.cbSearchAgency.DisplayMemberPath = "Name";

        }

        private void AddAgencytoPayment(ComboBox para)
        {
            string query = "SELECT * FROM AGENCY WHERE ISDELETE = 0";
            this.ListAgency = DataProvider.Instance.DB.Agencies.SqlQuery(query).ToList<Agency>();

            if (String.IsNullOrEmpty(para.Text))
            {
                MessageBox.Show("Vui lòng chọn đại lý");
            }
            else
            {
                int pos = para.SelectedIndex;
                int id = ListAgency[pos].ID;
                Agency item = DataProvider.Instance.DB.Agencies.Where(x => x.ID == id).First();

                this.HomeWindow.txbIDAgencyPayment.Text = item.ID.ToString();
                this.HomeWindow.txbAgencyinPayment.Text = item.Name.ToString();
                this.HomeWindow.txbAddressinPayment.Text = item.Address.ToString();
                this.HomeWindow.txbPhoneNumberinPayment.Text = item.PhoneNumber.ToString();
            }
        }

        private void LoadPayment(TextBox para)
        {
            if (!String.IsNullOrEmpty(para.Text))
            {
                long retainer = ConvertToNumber(para.Text);
                long total = ConvertToNumber(this.HomeWindow.TotalFeeofProductChosenPayment.Text);

                if (retainer < total)
                    this.HomeWindow.txbChangePayment.Text = SeparateThousands((total - retainer).ToString());
                else
                    para.Text = "0";
            }
            else
                para.Text = "0";
        }

        private void RemovefromListChosen(BusinessProductChosenUC para)
        {
            this.HomeWindow.stkListProductChosenBusiness.Children.Remove(para);

            int id = int.Parse(para.txbID.Text);

            Product item = DataProvider.Instance.DB.Products.Where(x => x.ID == id).First();
            ListProductChosen.Remove(item);
            LoadTotalofPayment();
            this.HomeWindow.txbChangePayment.Text = "";
            this.HomeWindow.txtRetainerPaymment.Text = "";
        }


        private void LoadTotalofPayment()
        {
            long total = 0;
            foreach (BusinessProductChosenUC item in this.HomeWindow.stkListProductChosenBusiness.Children)
            {
                total += ConvertToNumber(item.txbTotal.Text);
            }

            this.HomeWindow.TotalFeeofProductChosenPayment.Text = SeparateThousands(total.ToString());
        }

        private void LoadListChosen(BusinessProductUC para)
        {
            bool flag = true;
            int id = int.Parse(para.txbId.Text);

            if (ListProductChosen != null)
            {
                foreach (Product item in ListProductChosen)
                {
                    if (item.ID == id)
                    {
                        flag = false;
                        break;
                    }
                }
            }
            else
            {
                ListProductChosen = new List<Product>();
            }

            if (flag == true)
            {
                Product item = DataProvider.Instance.DB.Products.Where(x => x.ID == id).First();
                ListProductChosen.Add(item);
                BusinessProductChosenUC uc = new BusinessProductChosenUC();

                uc.txbID.Text = item.ID.ToString();
                uc.txbName.Text = item.Name.ToString();
                uc.txbPrice.Text = SeparateThousands(item.ExportPrice.Value.ToString());
                uc.txbUnit.Text = item.Unit;
                uc.txbAmount.Text = "1";
                uc.txbTotal.Text = SeparateThousands(item.ExportPrice.Value.ToString());

                this.HomeWindow.stkListProductChosenBusiness.Children.Add(uc);
            }
            else
            {
                foreach (BusinessProductChosenUC item in this.HomeWindow.stkListProductChosenBusiness.Children)
                {
                    if (item.txbID.Text == id.ToString())
                    {
                        int amount = int.Parse(item.txbAmount.Text) + 1;
                        long total = ConvertToNumber(item.txbPrice.Text) * amount;
                        item.txbAmount.Text = amount.ToString();
                        item.txbTotal.Text = SeparateThousands(total.ToString());
                    }
                }
            }
            LoadTotalofPayment();
            LoadPayment(this.HomeWindow.txtRetainerPaymment);
        }

        private void LoadBusiness(HomeWindow para)
        {
            this.HomeWindow = para;

            foreach (Product item in ListProduct)
            {
                BusinessProductUC uc = new BusinessProductUC();
                uc.txbId.Text = item.ID.ToString();
                uc.txbNameProductBusinessUC.Text = item.Name.ToString();
                uc.txbPriceProduct.Text = SeparateThousands(item.ExportPrice.Value.ToString());

                ImageBrush imageBrush = new ImageBrush();
                imageBrush.ImageSource = Converter.Instance.ConvertByteToBitmapImage(item.Image);

                uc.imgProduct.Background = imageBrush;
                para.stkListProductBusiness.Children.Add(uc);
            }
        }

        private String SeparateThousands(String txt)
        {
            if (!string.IsNullOrEmpty(txt))
            {
                System.Globalization.CultureInfo culture = new System.Globalization.CultureInfo("en-US");
                ulong valueBefore = ulong.Parse(ConvertToNumber(txt).ToString(), System.Globalization.NumberStyles.AllowThousands);
                txt = String.Format(culture, "{0:N0}", valueBefore);
            }
            return txt;
        }
    }
}
