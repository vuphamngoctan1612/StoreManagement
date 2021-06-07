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
using System.Data.SqlClient;
using System.Data;
using Microsoft;
using FastMember;

namespace StoreManagement.ViewModels
{
    public class BillViewModel : BaseViewModel
    {
        private string uid;
        private int status = 0;
        private long? total = 0;
        private long? pay = 0;


        public HomeWindow HomeWindow { get; set; }
        public ICommand OpenInvoiceWindowCommand { get; set; }
        public ICommand OpenReceiptWindowCommand { get; set; }
        public ICommand SeparateThousandsCommand { get; set; }
        public ICommand LoadBillCommand { get; set; }
        public ICommand LoadReceiptBillCommand { get; set; }
        public ICommand LoadStockReceiptCommnad { get; set; }
        public ICommand SwitchCommand { get; set; }
        public ICommand ExportExcelCommand { get; set; }
        public ICommand GetUidCommand { get; set; }
        public ICommand SearchAgencyCommand { get; set; }
        public ICommand ClearCommand { get; set; }

        public static void EnableCollectionSynchronization(System.Collections.IEnumerable collection, object context, System.Windows.Data.CollectionSynchronizationCallback synchronizationCallback) { }


        public BillViewModel()
        {
            LoadBillCommand = new RelayCommand<HomeWindow>((para) => true, (para) => LoadBill(para));
            LoadReceiptBillCommand = new RelayCommand<HomeWindow>((para) => true, (para) => LoadReceiptBill(para));
            LoadStockReceiptCommnad = new RelayCommand<HomeWindow>((para) => true, (para) => LoadStockReceipt(para));
            GetUidCommand = new RelayCommand<ComboBox>((para) => true, (para) => uid = para.Uid);
            SwitchCommand = new RelayCommand<HomeWindow>((para) => true, (para) => Switch(para));
            SearchAgencyCommand = new RelayCommand<HomeWindow>((para) => true, (para) => Search(para));
            OpenInvoiceWindowCommand = new RelayCommand<InvoiceUC>((para) => true, (para) => OpenInvoiceWindow(para));
            OpenReceiptWindowCommand = new RelayCommand<ReceiptBillUC>((para) => true, (para) => OpenReceiptWindow(para));
            ExportExcelCommand = new RelayCommand<HomeWindow>((para) => true, (para) => ExportExcel(para));
            ClearCommand = new RelayCommand<HomeWindow>((para) => true, (para) => Clear(para));

        }

        public void Clear(HomeWindow para)
        {
            total = 0;
            pay = 0;
        }

        private void ExportExcel(HomeWindow para)
        {
            if (status == 0)
            {
                MessageBox.Show("Click Releasing Bill, Receipt Bill or Stock Receipt first");
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
                DataTable data = new DataTable();
                if (status == 1)
                {
                    List<Invoice> invoices = DataProvider.Instance.DB.Invoices.ToList();
                    using (var reader = ObjectReader.Create(invoices))
                    {
                        data.Load(reader);
                    }
                    data.Columns.Remove("Agency");
                    data.Columns.Remove("InvoiceInfoes");
                }
                if (status == 2)
                {
                    List<Receipt> receipts = DataProvider.Instance.DB.Receipts.ToList();
                    using (var reader = ObjectReader.Create(receipts))
                    {
                        data.Load(reader);
                    }
                    data.Columns.Remove("Agency");

                }
                if (status == 3)
                {
                    List<StockReceipt> stockReceipts = DataProvider.Instance.DB.StockReceipts.ToList();
                    using (var reader = ObjectReader.Create(stockReceipts))
                    {
                        data.Load(reader);
                    }
                    data.Columns.Remove("StockReceiptInfoes");
                }
                worksheet = application.Worksheets.Add(misValue, misValue, misValue, misValue);
                worksheet.Name = "Bill";
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
                workbook.SaveAs(sfd.FileName);
            }
        }
        private void OpenReceiptWindow(ReceiptBillUC para)
        {
            int id = int.Parse(para.ReceiptID.Text);
            Receipt receipt = (Receipt)DataProvider.Instance.DB.Receipts.Where(x => x.ID == id).First();
            ReceiptBillWindow receiptBillWindow = new ReceiptBillWindow();
            receiptBillWindow.txtAgencyName.Text = receipt.Agency.Name;
            receiptBillWindow.txtPhoneNumber.Text = receipt.Agency.PhoneNumber;
            receiptBillWindow.txtAddress.Text = receipt.Agency.Address;
            receiptBillWindow.txtEmail.Text = receipt.Agency.Email;
            receiptBillWindow.txtProceed.Text = receipt.Message;
            receiptBillWindow.dateCheckout.Text = receipt.Date.ToString();
            receiptBillWindow.ShowDialog();
        }
        private void OpenInvoiceWindow(InvoiceUC para)
        {
            if (para.AgencyName.Text == "Our company")
            {
                int no = 1;
                StockReceipt stockReceipt = new StockReceipt();
                int id = int.Parse(para.InvoiceID.Text);
                stockReceipt = (StockReceipt)DataProvider.Instance.DB.StockReceipts.Where(x => x.ID == id).First();
                List<StockReceiptInfo> stockReceiptInfos = stockReceipt.StockReceiptInfoes.ToList();
                InvoiceBillWindow invoiceWindow = new InvoiceBillWindow();
                invoiceWindow.txbName.Text = "Our company";
                invoiceWindow.txbAddress.Text = "University of Infomation Technology";
                invoiceWindow.txbInvoiceID.Text = stockReceipt.ID.ToString();
                invoiceWindow.txbInvoiceDate.Text = stockReceipt.CheckIn.Value.ToShortDateString();
                foreach (StockReceiptInfo stockReceiptInfo in stockReceiptInfos)
                {
                    BillUC billUC = new BillUC();
                    billUC.ID.Text = no.ToString();
                    no++;
                    billUC.UnitName.Text = stockReceiptInfo.Product.Name.ToString();
                    billUC.UnitName.Text = stockReceiptInfo.Product.Name.ToString();
                    billUC.Unit.Text = stockReceiptInfo.Product.Unit.ToString();
                    billUC.Amount.Text = stockReceiptInfo.Amount.ToString();
                    billUC.Price.Text = stockReceiptInfo.Product.ExportPrice.ToString();
                    billUC.Total.Text = ConvertToString(stockReceiptInfo.Price);
                    invoiceWindow.stkListInvoiceInfos.Children.Add(billUC);
                }
                invoiceWindow.txbTotal.Text = ConvertToString(stockReceipt.Total);
                invoiceWindow.txbDebt.Visibility = System.Windows.Visibility.Hidden;
                invoiceWindow.txbPrepay.Visibility = System.Windows.Visibility.Hidden;
                invoiceWindow.textPre.Visibility = System.Windows.Visibility.Hidden;
                invoiceWindow.textPreVND.Visibility = System.Windows.Visibility.Hidden;
                invoiceWindow.textRest.Visibility = System.Windows.Visibility.Hidden;
                invoiceWindow.textRestVND.Visibility = System.Windows.Visibility.Hidden;
                invoiceWindow.ShowDialog();
            }
            else
            {
                int no = 1;
                Invoice invoice = new Invoice();
                int id = int.Parse(para.InvoiceID.Text);
                invoice = (Invoice)DataProvider.Instance.DB.Invoices.Where(x => x.ID == id).First();
                List<InvoiceInfo> invoiceInfos = invoice.InvoiceInfoes.ToList();
                InvoiceBillWindow invoiceWindow = new InvoiceBillWindow();
                invoiceWindow.txbName.Text = invoice.Agency.Name;
                invoiceWindow.txbAddress.Text = invoice.Agency.Address;
                invoiceWindow.txbPhone.Text = invoice.Agency.PhoneNumber;
                invoiceWindow.txbInvoiceID.Text = invoice.ID.ToString();
                invoiceWindow.txbInvoiceDate.Text = invoice.Checkout.Value.ToShortDateString();
                foreach (InvoiceInfo invoiceInfo in invoiceInfos)
                {
                    Product product = new Product();
                    BillUC billUC = new BillUC();
                    billUC.ID.Text = no.ToString();
                    no++;
                    billUC.UnitName.Text = invoiceInfo.Product.Name.ToString();
                    billUC.Unit.Text = invoiceInfo.Product.Unit.ToString();
                    billUC.Amount.Text = invoiceInfo.Amount.ToString();
                    billUC.Price.Text = invoiceInfo.Product.ExportPrice.ToString();
                    billUC.Total.Text = ConvertToString(invoiceInfo.Total);
                    invoiceWindow.stkListInvoiceInfos.Children.Add(billUC);
                }
                invoiceWindow.txbTotal.Text = ConvertToString(invoice.Total);
                invoiceWindow.txbPrepay.Text = ConvertToString((invoice.Total - invoice.Debt));
                invoiceWindow.txbDebt.Text = ConvertToString(invoice.Debt);
                invoiceWindow.ShowDialog();
            }
        }
        private void Search(HomeWindow para)
        {
            this.HomeWindow = para;
            foreach (InvoiceUC control in this.HomeWindow.stkBill.Children)
            {
                if (!control.AgencyName.Text.ToLower().Contains(this.HomeWindow.txtSearchAgencyinBill.Text))
                {
                    control.Visibility = Visibility.Collapsed;
                }
                else
                {
                    control.Visibility = Visibility.Visible;
                }
            }
            foreach (ReceiptBillUC control in this.HomeWindow.stkReceiptBill.Children)
            {
                if (!control.AgencyName.Text.ToLower().Contains(this.HomeWindow.txtSearchAgencyinBill.Text))
                {
                    control.Visibility = Visibility.Collapsed;
                }
                else
                {
                    control.Visibility = Visibility.Visible;
                }
            }
        }
        public void LoadBill(HomeWindow para)
        {
            this.HomeWindow = para;
            this.HomeWindow.stkBill.Children.Clear();
            List<Invoice> invoices = new List<Invoice>();
            invoices = DataProvider.Instance.DB.Invoices.ToList<Invoice>();
            foreach (Invoice invoice in invoices)
            {
                InvoiceUC invoiceUC = new InvoiceUC();
                invoiceUC.InvoiceID.Text = invoice.ID.ToString();
                invoiceUC.AgencyName.Text = invoice.Agency.Name.ToString();
                invoiceUC.CheckOut.Text = invoice.Checkout.Value.ToShortDateString();
                invoiceUC.Debt.Text = ConvertToString(invoice.Debt);
                total += invoice.Total;
                this.HomeWindow.stkBill.Children.Add(invoiceUC);
            }
            this.HomeWindow.textCollect.Text = ConvertToString(total);
        }
        public void LoadReceiptBill(HomeWindow para)
        {
            this.HomeWindow = para;
            this.HomeWindow.stkReceiptBill.Children.Clear();
            List<Receipt> receipts = new List<Receipt>();
            receipts = DataProvider.Instance.DB.Receipts.ToList<Receipt>();
            foreach (Receipt receipt in receipts)
            {
                ReceiptBillUC receiptBillUC = new ReceiptBillUC();
                receiptBillUC.ReceiptID.Text = receipt.ID.ToString();
                receiptBillUC.AgencyName.Text = receipt.Agency.Name.ToString();
                receiptBillUC.CheckOut.Text = receipt.Date.Value.ToShortDateString();
                receiptBillUC.Amount.Text = ConvertToString(receipt.Amount);
                this.HomeWindow.stkReceiptBill.Children.Add(receiptBillUC);
            }
        }
        public void LoadStockReceipt(HomeWindow para)
        {
            this.HomeWindow = para;
            this.HomeWindow.stkStockReceipt.Children.Clear();
            List<StockReceipt> stockReceipts = new List<StockReceipt>();
            stockReceipts = DataProvider.Instance.DB.StockReceipts.ToList<StockReceipt>();
            foreach (StockReceipt stockReceipt in stockReceipts)
            {
                InvoiceUC invoiceUC = new InvoiceUC();
                invoiceUC.InvoiceID.Text = stockReceipt.ID.ToString();
                invoiceUC.AgencyName.Text = "Our company";
                invoiceUC.CheckOut.Text = stockReceipt.CheckIn.Value.ToShortDateString();
                invoiceUC.Debt.Text = ConvertToString(stockReceipt.Total);
                pay += stockReceipt.Total;
                this.HomeWindow.stkStockReceipt.Children.Add(invoiceUC);
            }
            this.HomeWindow.textPay.Text = "-" + ConvertToString(pay);
        }
        private void Switch(HomeWindow para)
        {
            if (para.comboBoxBill.SelectedIndex == 1)
            {
                status = 1;
                para.InvoiceTable.Visibility = System.Windows.Visibility.Visible;
                para.stkBill.Visibility = System.Windows.Visibility.Visible;
                para.stkReceiptBill.Visibility = System.Windows.Visibility.Hidden;
                para.stkStockReceipt.Visibility = System.Windows.Visibility.Hidden;
                para.ColumnHeaderBill.Visibility = System.Windows.Visibility.Visible;
                para.ScrollInvoice.Visibility = System.Windows.Visibility.Visible;
                para.ScrollReceipt.Visibility = System.Windows.Visibility.Hidden;
                para.ScrollStockReceipt.Visibility = System.Windows.Visibility.Hidden;
                para.LastBlock.Text = "Debt";
            }
            if (para.comboBoxBill.SelectedIndex == 2)
            {
                status = 2;
                para.InvoiceTable.Visibility = System.Windows.Visibility.Visible;
                para.stkBill.Visibility = System.Windows.Visibility.Hidden;
                para.stkReceiptBill.Visibility = System.Windows.Visibility.Visible;
                para.stkStockReceipt.Visibility = System.Windows.Visibility.Hidden;
                para.ColumnHeaderBill.Visibility = System.Windows.Visibility.Visible;
                para.ScrollReceipt.Visibility = System.Windows.Visibility.Visible;
                para.ScrollInvoice.Visibility = System.Windows.Visibility.Hidden;
                para.ScrollStockReceipt.Visibility = System.Windows.Visibility.Hidden;
                para.LastBlock.Text = "Amount";
            }
            if (para.comboBoxBill.SelectedIndex == 0)
            {
                status = 3;
                para.InvoiceTable.Visibility = System.Windows.Visibility.Visible;
                para.stkBill.Visibility = System.Windows.Visibility.Hidden;
                para.stkReceiptBill.Visibility = System.Windows.Visibility.Hidden;
                para.stkStockReceipt.Visibility = System.Windows.Visibility.Visible;
                para.ColumnHeaderBill.Visibility = System.Windows.Visibility.Visible;
                para.ScrollInvoice.Visibility = System.Windows.Visibility.Hidden;
                para.ScrollReceipt.Visibility = System.Windows.Visibility.Hidden;
                para.ScrollStockReceipt.Visibility = System.Windows.Visibility.Visible;
                para.LastBlock.Text = "Total";
            }

        }
    }
}
