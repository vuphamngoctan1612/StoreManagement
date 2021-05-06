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
    public class BillViewModel : BaseViewModel
    {
        private string uid;

        public HomeWindow HomeWindow { get; set; }
        public ICommand OpenInvoiceWindowCommand { get; set; }
        public ICommand OpenReceiptWindowCommand { get; set; }
        public ICommand SeparateThousandsCommand { get; set; }
        public ICommand LoadBillCommand { get; set; }
        public ICommand LoadReceiptBillCommand { get; set; }
        public ICommand SwitchCommand { get; set; }
        public ICommand ExportBillExcelCommand { get; set; }
        public ICommand ExportReceiptExcelCommand { get; set; }
        public ICommand GetUidCommand { get; set; }
        public ICommand SearchAgencyCommand { get; set; }

        public BillViewModel()
        {
            LoadBillCommand = new RelayCommand<HomeWindow>((para) => true, (para) => LoadBill(para));
            LoadReceiptBillCommand = new RelayCommand<HomeWindow>((para) => true, (para) => LoadReceiptBill(para));
            GetUidCommand = new RelayCommand<Button>((para) => true, (para) => uid = para.Uid);
            SwitchCommand = new RelayCommand<HomeWindow>((para) => true, (para) => Switch(para));
            SearchAgencyCommand = new RelayCommand<HomeWindow>((para) => true, (para) => Search(para));
            OpenInvoiceWindowCommand = new RelayCommand<InvoiceUC>((para) => true, (para) => OpenInvoiceWindow(para));
            OpenReceiptWindowCommand = new RelayCommand<ReceiptBillUC>((para) => true, (para) => OpenReceiptWindow(para));
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
        private void OpenInvoiceWindow (InvoiceUC para)
        {
            int no = 1;
            long? total = 0;
            Invoice invoice = new Invoice();
            int id = int.Parse(para.InvoiceID.Text);
            invoice = (Invoice)DataProvider.Instance.DB.Invoices.Where(x => x.ID == id).First();
            List<InvoiceInfo> invoiceInfos = invoice.InvoiceInfoes.ToList();
            InvoiceWindow invoiceWindow = new InvoiceWindow();
            invoiceWindow.txbName.Text = invoice.Agency.Name;
            invoiceWindow.txbAddress.Text = invoice.Agency.Address;
            invoiceWindow.txbPhone.Text = invoice.Agency.PhoneNumber;
            invoiceWindow.txbInvoiceID.Text = invoice.ID.ToString();
            invoiceWindow.txbInvoiceDate.Text = invoice.Checkout.ToString();
            foreach (InvoiceInfo invoiceInfo in invoiceInfos)
            {
                Product product = new Product();
                BillUC billUC = new BillUC();
                billUC.ID.Text = no.ToString();
                billUC.UnitName.Text = invoiceInfo.Product.Name.ToString();
                billUC.Unit.Text = invoiceInfo.Product.Unit.ToString();
                billUC.Amount.Text = invoiceInfo.Amount.ToString();
                billUC.Price.Text = invoiceInfo.Product.ExportPrice.ToString();
                billUC.Total.Text = invoiceInfo.Total.ToString();
                total += invoiceInfo.Total;
                invoiceWindow.stkListInvoiceInfos.Children.Add(billUC);
            }
            invoiceWindow.txbTotal.Text = total.ToString();
            invoiceWindow.txbPrepay.Text = (total - invoice.Debt).ToString();
            invoiceWindow.txbDebt.Text = invoice.Debt.ToString();
            invoiceWindow.ShowDialog();
        }
        private void Search(HomeWindow para)
        {
            this.HomeWindow = para;
            foreach (InvoiceUC control in this.HomeWindow.stkBill.Children)
            {
                if (!control.AgencyName.Text.ToLower().Contains(this.HomeWindow.txtSearchAgency.Text))
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
                if (!control.AgencyName.Text.ToLower().Contains(this.HomeWindow.txtSearchAgency.Text))
                {
                    control.Visibility = Visibility.Collapsed;
                }
                else
                {
                    control.Visibility = Visibility.Visible;
                }
            }    
        }
        private void LoadBill(HomeWindow para)
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
                invoiceUC.CheckOut.Text = invoice.Checkout.ToString();
                invoiceUC.Debt.Text = invoice.Debt.ToString();
                this.HomeWindow.stkBill.Children.Add(invoiceUC);
            }    
        }
        private void LoadReceiptBill(HomeWindow para)
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
                receiptBillUC.CheckOut.Text = receipt.Date.ToString();
                receiptBillUC.Amount.Text = receipt.Amount.ToString();
                this.HomeWindow.stkReceiptBill.Children.Add(receiptBillUC);
            }    
        }
        private void Switch(HomeWindow para)
        {
            int index = int.Parse(uid);
            switch (index)
            {
                case 1:
                    para.InvoiceTable.Visibility = System.Windows.Visibility.Visible;
                    para.textReleasingBill.Visibility = System.Windows.Visibility.Visible;
                    para.stkBill.Visibility = System.Windows.Visibility.Visible;
                    para.textReceiptBill.Visibility = System.Windows.Visibility.Hidden;
                    para.stkReceiptBill.Visibility = System.Windows.Visibility.Hidden;
                    para.LastBlock.Text = "Debt";
                    break;
                case 2:
                    para.InvoiceTable.Visibility = System.Windows.Visibility.Visible;
                    para.textReleasingBill.Visibility = System.Windows.Visibility.Hidden;
                    para.stkBill.Visibility = System.Windows.Visibility.Hidden;
                    para.textReceiptBill.Visibility = System.Windows.Visibility.Visible;
                    para.stkReceiptBill.Visibility = System.Windows.Visibility.Visible;
                    para.LastBlock.Text = "Amount";
                    break;
            }
        }
    }
}
